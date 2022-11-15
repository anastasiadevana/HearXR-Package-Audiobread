using System;
using System.Collections.Generic;
using HearXR.Audiobread.SoundProperties;
using UnityEngine;

namespace HearXR.Audiobread
{
    public abstract class SoundGeneratorWrapper<TDefinition, TSelf, TGenerator> : Sound<TDefinition, TSelf>, ISoundGeneratorWrapper
        where TDefinition : class, ISoundDefinition
        where TSelf : Sound
        where TGenerator : Sound, ISoundGenerator
    {
        #region Protected Fields
        protected readonly List<Guid> _nonStoppedGenerators = new List<Guid>();
        protected TGenerator _primedGenerator;
        protected TGenerator _lastPlayedGenerator;
        #endregion
        
        #region ISoundGeneratorWrapper Properties
        public List<ISoundGenerator> Generators => _generators as List<ISoundGenerator>;
        protected readonly List<TGenerator> _generators = new List<TGenerator>();
        #endregion

        #region Sound Method Overrides
        internal override void DoUpdate()
        {
            base.DoUpdate();
            UpdateSoundMultiple(GetGenerators());
            _primedGenerator?.Update();
        }
        
        protected override void PostSetSoundSourceObject()
        {
            base.PostSetSoundSourceObject();
            SetSoundSourceObjectMultiple(GetGenerators(), SoundSourceObject);
            if (HavePrimedGenerator())
            {
                _primedGenerator.SoundSourceObject = SoundSourceObject;
            }
        }
        
        protected override void PostSetMidiNoteInfo()
        {
            base.PostSetMidiNoteInfo();
            SetMidiNoteInfoMultiple(GetGenerators(), MidiNoteInfo);
            if (HavePrimedGenerator())
            {
                _primedGenerator.MidiNoteInfo = MidiNoteInfo;
            }
        }
        #endregion

        #region Sound<TDefinition, TSelf> Overrides
        protected override void DoRefreshSoundDefinition()
        {
            base.DoRefreshSoundDefinition();
            RefreshSoundDefinitionMultiple(GetGenerators());
            if (HavePrimedGenerator())
            {
                _primedGenerator.RefreshSoundDefinition();   
            }
        }
        
        protected override void DoReleaseResources()
        {
            base.DoReleaseResources();
            ReleaseResourcesMultiple(GetGenerators());
        }
        
        /// <summary>
        /// Since the SoundGeneratorWrapper and the actual SoundGenerator are using the same SoundDefinition,
        /// we don't want to apply the various sound properties (i.e. volume) calculation twice.
        /// </summary>
        /// <param name="previousParent"></param>
        /// <param name="newParent"></param>
        protected override void PostSetParent(ISound previousParent, ISound newParent)
        {
            base.PostSetParent(previousParent, newParent);
            for (var i = 0; i < _generators.Count; ++i)
            {
                _generators[i].ParentSound = newParent;
            }
        
            if (HavePrimedGenerator())
            {
                _primedGenerator.ParentSound = newParent;
            }
        }
        #endregion
        
        #region Sound Abstract Methods
        protected override void DoStop(StopSoundFlags stopSoundFlags) { StopMultiple(GetGenerators(), stopSoundFlags); }
        protected override void DoPause() { PauseMultiple(GetGenerators()); }
        protected override void DoResume() { ResumeMultiple(GetGenerators()); }
        protected override void DoMute() { MuteMultiple(GetGenerators()); }
        protected override void DoUnmute() { UnmuteMultiple(GetGenerators()); }
        #endregion
        
        #region Overrideable Methods
        protected virtual void DoOnChildBeforeEnded(ISound child, double endTime, int nonStoppedChildrenLeft) {}
        protected virtual void PostPlaybackEndedGetReadyToPlayAgain() {}
        protected virtual void DoOnSelfEnded()
        {
            if (IsPersistent() || HasParent)
            {
                PostPlaybackEndedGetReadyToPlayAgain();
            }
            else if (Registered)
            {
                ReleaseAndUnregisterSelf();
            }
        }
        #endregion
        
        #region Protected Methods
        protected virtual void UpdatePlaybackState()
        {
            lock (_nonStoppedGenerators)
            {
                var newState = Audiobread.GetPlaybackStateFromChildren(PlaybackState, GetGenerators(), _nonStoppedGenerators.Count);
                if (newState != PlaybackState)
                {
                    SetPlaybackState(newState);
                }
            }
        }

        private void SubscribeToChildEvents(ref ISound child)
        {
            child.OnBegan += OnChildBegan;
            child.OnEnded += OnChildEnded;
            child.OnScheduled += OnChildScheduled;
            child.OnBeforeEnded += OnChildBeforeEnded;
        }

        // TODO: When does this get called?
        private void UnsubscribeFromChildEvents(ref ISound child)
        {
            child.OnBegan -= OnChildBegan;
            child.OnEnded -= OnChildEnded;
            child.OnScheduled -= OnChildScheduled;
            child.OnBeforeEnded -= OnChildBeforeEnded;
        }

        protected bool HavePrimedGenerator()
        {
            return !EqualityComparer<TGenerator>.Default.Equals(_primedGenerator, default);
        }
        
        protected bool TryUseExistingGenerator()
        {
            if (_generators.Count == 0) return false;

            for (int i = 0; i < _generators.Count; ++i)
            {
                if (_generators[i].IsReadyToPlay())
                {
                    _primedGenerator = _generators[i];
                    _generators.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }
        #endregion
        
        #region Event Handlers
        protected void OnChildScheduled(ISound child, double desiredStartTime)
        {
            var previousState = PlaybackState;

            // Make sure we don't have a duplicate.
            lock (_nonStoppedGenerators)
            {
                if (!_nonStoppedGenerators.Contains(child.Guid))
                {
                    _nonStoppedGenerators.Add(child.Guid);
                }
                UpdatePlaybackState();
            }
            
            if (PlaybackState == previousState) return;
            if (previousState == PlaybackState.Stopped)
            {
                InvokeOnScheduled(this, desiredStartTime);
            }
        }
        
        protected void OnChildBegan(ISound child, double startTime)
        {
            // TODO: Do we need to pass BOTH this and child?
            InvokeOnChildBegan(child, startTime);
            
            // TODO: SoundModule hookup.
            // Tell the volume calculator to start fading in.
            // ((VolumeCalculator) _calculators[_volumeProperty]).OnSoundBegan(); 
            
            var previousState = PlaybackState;
            
            // Make sure we don't have a duplicate.
            lock (_nonStoppedGenerators)
            {
                if (!_nonStoppedGenerators.Contains(child.Guid))
                {
                    _nonStoppedGenerators.Add(child.Guid);
                }
                UpdatePlaybackState();
            }
            if (PlaybackState == previousState) return;
            if (PlaybackState != PlaybackState.Playing) return;
            if (previousState == PlaybackState.Stopped || previousState == PlaybackState.PlayInitiated)
            {
                InvokeOnBegan(this, startTime);
            }
        }
        
        protected void OnChildBeforeEnded(ISound child, double endTime)
        {
            int nonStoppedChildrenLeft = 0;
            lock (_nonStoppedGenerators)
            {
                UpdatePlaybackState();
                nonStoppedChildrenLeft = _nonStoppedGenerators.Count;
            }

            DoOnChildBeforeEnded(child, endTime, nonStoppedChildrenLeft);

            if (nonStoppedChildrenLeft == 1)
            {
                InvokeOnBeforeEnded(this, endTime);
            }
        }
        
        protected virtual void OnChildEnded(ISound child)
        {
            PlaybackState previousState = PlaybackState;
            lock (_nonStoppedGenerators)
            {
                _nonStoppedGenerators.Remove(child.Guid);
                UpdatePlaybackState();
            }
            
            if (PlaybackState == previousState) return;
            if (PlaybackState != PlaybackState.Stopped) return;
            
            // THIS PART IS NOT IDENTICAL
            DoOnSelfEnded();
            // END NOT IDENTICAL
            
            InvokeOnEnded(this);
        }
        #endregion
        
        #region Abstract Methods
        protected abstract IReadOnlyList<ISound> GetGenerators();
        #endregion
    }
}