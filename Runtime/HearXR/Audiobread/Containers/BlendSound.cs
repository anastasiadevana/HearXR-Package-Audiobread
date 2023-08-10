using UnityEngine;

namespace HearXR.Audiobread
{
    public class BlendSound : BaseContainerSound<BlendSoundDefinition, BlendSound>
    {
        #region Private Fields
        private CoreSchedulerSoundProcessor _schedulerSoundProcessor;
        #endregion

        #region Sound Methods Overrides
        protected override bool CanPlay(PlaySoundFlags playSoundFlags = PlaySoundFlags.None)
        {
            if (!base.CanPlay(playSoundFlags)) return false;

            if (!_schedulerSoundProcessor.CanPlay(playSoundFlags)) return false;

            if (PrepareChildren() && ChildrenAreValid())
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

            // Remove the "play next" flag - because it's play next just for this sound, not for the child.
            playFlags &= ~PlaySoundFlags.PlayNext;
            
            // Debug.Log("Play blend sounds!");
            
            for (var i = 0; i < _children.Length; ++i)
            {
                if (_instancePlaybackInfo.scheduledStart)
                {
                    _children[i].PlayScheduled(_instancePlaybackInfo.startTime, playFlags);
                }
                else
                {
                    _children[i].Play(playFlags);
                }
            }

            SetStatus(SoundStatus.Paused, false);
            
            if (_instancePlaybackInfo.scheduledEnd)
            {
                Debug.LogWarning("TODO: Container sounds don't support duration yet. Use Stop() instead.");
            }
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
            if (!hasScheduler)
            {
                ResetStatus(SoundStatus.Invalid);
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
            base.DoOnChildBeforeEnded(child, endTime, nonStoppedChildrenLeft);
            
            if (_schedulerSoundProcessor.PlayMore)
            {
                // Debug.Log("Play blend more!");
                
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

        #region Private Methods
        private bool PrepareChildren()
        {
            for (var i = 0; i < _children.Length; ++i)
            {
                var nextChild = (ISoundInternal) _children[i];
                if (nextChild == default || !nextChild.IsValid())
                {
                    Debug.LogError($"HEAR XR: We don't have the next child.");
                    return false;
                }
                nextChild.PrepareToPlay();
            }

            return true;
        }
        
        private bool ChildrenAreValid()
        {
            for (var i = 0; i < _children.Length; ++i)
            {
                if (!_children[i].IsValid()) return false;
            }

            return true;
        }
        #endregion
    }
}