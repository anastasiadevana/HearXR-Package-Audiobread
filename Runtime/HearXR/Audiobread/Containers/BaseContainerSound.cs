using System;
using System.Collections.Generic;
using UnityEngine;

namespace HearXR.Audiobread
{
    public abstract class BaseContainerSound<TDefinition, TSelf> : Sound<TDefinition, TSelf>, IContainerSound
        where TDefinition : class, IContainerSoundDefinition
        where TSelf : Sound
    {
        #region Private Fields
        protected readonly List<Guid> _nonStoppedChildren = new List<Guid>();
        #endregion
        
        #region ISoundContainer Properties
        public ISound[] Children => _children;
        protected ISound[] _children;
        
        public bool IsTopParent => ParentSound == null;
        #endregion

        #region Sound Method Overrides
        internal override void DoUpdate()
        {
            base.DoUpdate();
            UpdateSoundMultiple(GetChildren());
        }
        
        protected override void PostSetSoundSourceObject()
        {
            base.PostSetSoundSourceObject();
            SetSoundSourceObjectMultiple(GetChildren(), SoundSourceObject);
        }

        protected override void PostSetMidiNoteInfo()
        {
            base.PostSetMidiNoteInfo();
            SetMidiNoteInfoMultiple(GetChildren(), MidiNoteInfo);
        }
        #endregion

        #region Sound<TDefinition, TSelf> Overrides
        protected override void ParseSoundDefinition()
        {
            base.ParseSoundDefinition();
            ParseChildrenFromDefinition();
        }

        protected override void DoRefreshSoundDefinition()
        {
            base.DoRefreshSoundDefinition();
            RefreshSoundDefinitionMultiple(GetChildren());
        }
        
        protected override void DoReleaseResources()
        {
            base.DoReleaseResources();
            ReleaseResourcesMultiple(GetChildren());
        }
        #endregion
        
        #region Sound Abstract Methods
        protected override void DoStop(StopSoundFlags stopSoundFlags) { StopMultiple(GetChildren(), stopSoundFlags); }
        protected override void DoPause() { PauseMultiple(GetChildren()); }
        protected override void DoResume() { ResumeMultiple(GetChildren()); }
        protected override void DoMute() { MuteMultiple(GetChildren()); }
        protected override void DoUnmute() { UnmuteMultiple(GetChildren()); }
        #endregion
        
        #region Overrideable Methods
        protected virtual IReadOnlyList<ISound> GetChildren()
        {
            return _children;
        }
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
        protected void SetChildren(ISound[] children)
        {
            _children = children;
            _hasChildren = true;
            for (int i = 0; i < _children.Length; i++)
            {
                var child = _children[i];
                child.ParentSound = this;
            }
        }
        
        protected virtual void UpdatePlaybackState()
        {
            lock (_nonStoppedChildren)
            {
                var newState = Audiobread.GetPlaybackStateFromChildren(PlaybackState, GetChildren(), _nonStoppedChildren.Count);
                if (newState != PlaybackState)
                {
                    SetPlaybackState(newState);
                }
            }
        }
        
        protected virtual void ParseChildrenFromDefinition()
        {
            int definitionChildCount = _soundDefinition.ChildCount;
            if (definitionChildCount == 0)
            {
                ResetStatus(SoundStatus.Invalid);
                Debug.LogError($"HEAR XR: Nested sound has no children.");
                return;
            }

            bool shouldInitChildren = true;
            if (_children != null && _children.Length > 0)
            {
                if (_children.Length == definitionChildCount)
                {
                    shouldInitChildren = false;
                }
                else
                {
                    ResetStatus(SoundStatus.Invalid);
                    Debug.LogError("TODO: Already have children. We should clean up first!!! Like, unsubscribe from events and such as.");
                    return;
                }
            }

            if (!shouldInitChildren) return;
            
            var childDefinitions = _soundDefinition.GetChildren();
            ISound[] children = new ISound[childDefinitions.Length];
            for (int i = 0; i < childDefinitions.Length; ++i)
            {
                // Debug.Log(childDefinitions[i].Name);
                children[i] = CreateSound(childDefinitions[i]);
                SubscribeToChildEvents(ref children[i]);
            }
            SetChildren(children);
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
        #endregion
        
        #region Event Handlers
        protected void OnChildScheduled(ISound child, double desiredStartTime)
        {
            PlaybackState previousState = PlaybackState;

            // Make sure we don't have a duplicate.
            lock (_nonStoppedChildren)
            {
                if (!_nonStoppedChildren.Contains(child.Guid))
                {
                    _nonStoppedChildren.Add(child.Guid);
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
            
            PlaybackState previousState = PlaybackState;
            
            // Make sure we don't have a duplicate.
            lock (_nonStoppedChildren)
            {
                if (!_nonStoppedChildren.Contains(child.Guid))
                {
                    _nonStoppedChildren.Add(child.Guid);
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
            lock (_nonStoppedChildren)
            {
                UpdatePlaybackState();
                nonStoppedChildrenLeft = _nonStoppedChildren.Count;
            }

            DoOnChildBeforeEnded(child, endTime, nonStoppedChildrenLeft);
        }
        
        protected virtual void OnChildEnded(ISound child)
        {
            PlaybackState previousState = PlaybackState;
            lock (_nonStoppedChildren)
            {
                _nonStoppedChildren.Remove(child.Guid);
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
    }
}