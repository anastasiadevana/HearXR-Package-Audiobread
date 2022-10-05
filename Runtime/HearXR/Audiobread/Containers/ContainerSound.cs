using System;
using UnityEngine;

namespace HearXR.Audiobread
{
    public class ContainerSound : BaseContainerSound<ContainerSoundDefinition, ContainerSound>
    {
        #region Private Fields
        // private int _childIndex = -1;
        private ISoundInternal _child;
        private CoreSchedulerSoundProcessor _schedulerSoundProcessor;
        private CoreChildPickerSoundProcessor _childPickerProcessor;
        #endregion
        
        // #region Events
        // public event Action OnContainerBeforeFirstPlay;
        // #endregion
        
        #region Sound Methods Overrides
        protected override bool CanPlay(PlaySoundFlags playSoundFlags = PlaySoundFlags.None)
        {
            if (!base.CanPlay(playSoundFlags)) return false;
            
            // TODO: The throttle bit should be higher up.
            // if (_soundDefinition.Throttle && !_soundDefinition.CanPlay(playSoundFlags))
            // {
            //     return false;
            // }

            if (!_schedulerSoundProcessor.CanPlay(playSoundFlags)) return false;
            
            if (_child != null)
            {
                if (_child.IsValid()) return true;
                Debug.LogWarning($"HEAR XR: Unable to play {this} because child is invalid.");
                return false;
            }
            
            if (PrepareChild() && _child.IsValid())
            {
                return true;
            }
            
            Debug.LogWarning($"HEAR XR: Unable to play {this} because unable to prepare child.");
            return false;
        }
        
        protected override void DoPlay(PlaySoundFlags playFlags)
        {
            var playNext = HasPlayFlag(playFlags, PlaySoundFlags.PlayNext);
            
            // If the sound is already playing or stopping, and we're asked to play again, stop everything first.
            if (IsPlayingOrStopping() && !playNext)
            {
                Debug.Log("Stopping all children before playing again");
                StopMultiple(_children, StopSoundFlags.Instant);
            }

            _schedulerSoundProcessor.ProcessSchedulingBeforePlay(ref _instancePlaybackInfo, playFlags);
            
            ISound child = _child;
            _child = null;

            if (!playNext && !IsPlaying())
            {
                // TODO: SoundModule hookup, specifically volume fade-in. Also move elsewhere?
                // ((VolumeCalculator) _calculators[_volumeProperty]).PrepareToPlay(playFlags);   
            }

            // Remove the "play next" flag - because it's play next just for this sound, not for the child.
            playFlags &= ~PlaySoundFlags.PlayNext;
            
            if (_instancePlaybackInfo.scheduledStart)
            {
                child.PlayScheduled(_instancePlaybackInfo.startTime, playFlags);
            }
            else
            {
                child.Play(playFlags);
            }

            SetStatus(SoundStatus.Paused, false);
            
            // TODO: Handle throttling.
            // TODO: This should go higher.
            // if (_soundDefinition.Throttle)
            // {
            //     _soundDefinition.OnPlayed();
            // }
        }
        
        protected override void PostSetSoundDefinition(InitSoundFlags initSoundFlags = InitSoundFlags.None)
        {
            base.PostSetSoundDefinition(initSoundFlags);
            if (_soundDefinition.ChildSchedulerDefinition == null)
            {
                ResetStatus(SoundStatus.Invalid);
            }
        }

        protected override void PostInitModules(InitSoundFlags initSoundFlags = InitSoundFlags.None)
        {
            base.PostInitModules(initSoundFlags);
            var hasScheduler = _soundModuleGroupProcessor.TryGetProcessor(out _schedulerSoundProcessor);
            var hasChildPicker = _soundModuleGroupProcessor.TryGetProcessor(out _childPickerProcessor);
            if (!hasScheduler || !hasChildPicker)
            {
                ResetStatus(SoundStatus.Invalid);
            }
        }
        
        protected override void PostSetSoundSourceObject()
        {
            base.PostSetSoundSourceObject();
            if (_child != null)
            {
                _child.SoundSourceObject = SoundSourceObject;
            }
        }
        
        protected override void PostPlaybackEndedGetReadyToPlayAgain()
        {
            base.PostPlaybackEndedGetReadyToPlayAgain();

            _schedulerSoundProcessor.InitPlaybackValues();
        }
        #endregion
        
        #region BaseContainerSound Overrides
        protected override void DoOnChildBeforeEnded(ISound child, double endTime, int nonStoppedChildrenLeft)
        {
            // Debug.Log("CONTAINER do on child before ended");
            
            base.DoOnChildBeforeEnded(child, endTime, nonStoppedChildrenLeft);
            
            // Debug.Log($"OnChildBeforeEnded {child}");
            
            if (_schedulerSoundProcessor.PlayMore)
            {
                // TODO: This is probably the best place to do this...
                // TODO: If the time before the next child is too long, instead of calling PlayScheduled here, do some kind of other waiting.
                PlayScheduled(endTime, PlaySoundFlags.PlayNext);
            }
            else if (nonStoppedChildrenLeft == 1)
            {
                InvokeOnBeforeEnded(this, endTime);
            }
        }
        #endregion
        
        #region Sound<TDefinition, TSelf> Overrides
        protected override void DoRefreshSoundDefinition()
        {
            base.DoRefreshSoundDefinition();
            _child?.RefreshSoundDefinition();
        }
        #endregion
        
        #region Private Methods
        private bool PrepareChild()
        {
            if (_child != default)
            {
                Debug.Log($"HEAR XR: {this} already has a prepared child.");
                return true;
            }

            var childIndex = _schedulerSoundProcessor.ChildIndex;
            var nextChildIndex = _childPickerProcessor.GetNextChildIndex(childIndex);
            _schedulerSoundProcessor.ChildIndex = nextChildIndex;

            _child = (ISoundInternal) _children[nextChildIndex];
            
            if (_child == default || !_child.IsValid())
            {
                Debug.LogError($"HEAR XR: We don't have the next child.");
                return false;
            }
            
            _child.PrepareToPlay();

            return true;
        }
        #endregion
    }
}