using HearXR.Audiobread.SoundProperties;
using UnityEngine;

namespace HearXR.Audiobread
{
    // TODO: THIS IS NO LONGER USED. DELETE.
    public class SequenceSound : BaseContainerSound<SequenceSoundDefinition, SequenceSound>
    {
        #region Protected Fields
        protected bool _hasFadeIn;
        protected Fade _fadeIn;
        protected bool _hasFadeOut;
        protected Fade _fadeOut;
        private SpatializationType _spatializationType;
        
        private int _childIndex = -1;
        private ISoundInternal _child;
        private new SequenceSoundDefinition _soundDefinition;
        protected bool _scheduleChildren;
        // TODO: Make a read-only property for inheriting classes.
        protected bool _playMore;
        private ParentSoundPlaybackOrder _playbackOrder;
        private ParentSoundRepeatType _repeatType;
        private bool _limitedRepeats;
        private int _repeatsRemaining;
        #endregion

        #region Sound Methods Overrides

        protected override void PostSetSoundDefinition(InitSoundFlags initSoundFlags = InitSoundFlags.None)
        {
            base.PostSetSoundDefinition(initSoundFlags);

            // TODO: If we know about fades here, do we need to put the same logic into the VolumeCalculator?
            // _hasFadeIn = _soundDefinition.FadeInDefinition.duration > Mathf.Epsilon;
            // _fadeIn = (_hasFadeIn) ? _soundDefinition.FadeInDefinition : null;
            // _hasFadeOut = _soundDefinition.FadeOutDefinition.duration > Mathf.Epsilon;
            // _fadeOut = (_hasFadeOut) ? _soundDefinition.FadeOutDefinition : null;
            
            _soundDefinition = base._soundDefinition;
        }
        
        // NOTE: Moved to SoundModuleProcessor
        // protected override void PostInitSoundProperties(InitSoundFlags initSoundFlags = InitSoundFlags.None)
        // {
        //     base.PostInitSoundProperties(initSoundFlags);
        //     
        //     // Add fades to the volume calculator.
        //     ((VolumeCalculator) _calculators[_volumeProperty]).SetFades(_soundDefinition.FadeInDefinition, _soundDefinition.FadeOutDefinition);
        //     // TODO: Unsubscribe from this at some point?
        //     ((VolumeCalculator) _calculators[_volumeProperty]).OnFadeFinished += delegate(Fade.Direction direction)
        //     {
        //         // TODO: Handle fade in finished.
        //         if (direction == Fade.Direction.Out)
        //         {
        //             OnFadeOutFinished();
        //         }
        //     };
        // }

        // protected override bool CanStop(StopSoundFlags stopSoundFlags = StopSoundFlags.None)
        // {
        //     //return base.CanStop(stopSoundFlags);
        //     if (!base.CanStop(stopSoundFlags)) return false;
        //
        //     if (HasStopFlag(stopSoundFlags, StopSoundFlags.Instant)) return true;
        //     
        //     if (!_soundModuleGroupProcessor.CanStopNow(stopSoundFlags, out var blockingSoundModule))
        //     {
        //         _soundModuleGroupProcessor.RequestStop(stopSoundFlags, ref blockingSoundModule, HandleStopBlockReleased);
        //         return false;
        //     }
        //     return true;
        //
        //     // NOTE: Moved to SoundModuleProcessor
        //     //((VolumeCalculator) _calculators[_volumeProperty]).OnStopSound(stopSoundFlags);
        //     //return !_hasFadeOut || HasStopFlag(stopSoundFlags, StopSoundFlags.Instant);
        //     // return HasStopFlag(stopSoundFlags, StopSoundFlags.Instant);
        //     // END
        // }
        #endregion
        
        #region Sound<TDefinition> Method Overrides
        // protected override void ParseSoundDefinition()
        // {
        //     base.ParseSoundDefinition();
        //     SetSpatialization();
        //     SetPlaybackSettings();
        // }
        #endregion
        
        #region Private Methods
        // private void SetSpatialization()
        // {
        //     // TODO:
        //     // _spatializationType = _soundDefinition.SpatializationType;
        // }
        #endregion

        // #region Sound Abstract Properties
        // public override bool CanBeRegistered => true;
        // #endregion

        // public int RepeatsRemaining => _repeatsRemaining;

        #region Sound Abstract Methods
        
        // NOTE: Moved to ContainerSound.
        
        protected override void DoPlay(PlaySoundFlags playFlags)
        {
            // bool playNext = HasPlayFlag(playFlags, PlaySoundFlags.PlayNext);
            //
            // // If the sound is already playing or stopping, and we're asked to play again, stop everything first.
            // if ((IsPlaying() || PlaybackState == PlaybackState.StopInitiated) && !playNext)
            // {
            //     Debug.Log("Stopping all children before playing again");
            //     StopMultiple(_children, StopSoundFlags.Instant);
            // }
            //
            // // Even if PlayScheduled wasn't called explicitly, but we have children, use PlayScheduled anyway.
            // if (!scheduledStart && (_scheduleChildren || playNext))
            // {
            //     scheduledStart = true;
            //     startTime = GetScheduledStartTime();
            //     //Debug.Log($"Get scheduledStart start time {startTime} current time: {AudioSettings.dspTime}");
            // }
            //
            // // If we're playing the next child, add the time between to the start time.
            // // TODO: Go into a different / "waiting" mode instead.
            // if (playNext)
            // {
            //     // TODO: SoundModule hookup.
            //     // startTime += _calculators[_timeBetweenProperty].ValueContainer.FloatValue;
            // }
            //
            // // TODO: This should go one higher.
            // if (!Registered && !HasParent)
            // {
            //     RegisterSelf();
            // }
            //
            // DecrementRepeatsRemaining(_childIndex);
            // ISound child = _child;
            // _child = null;
            //
            // if (!playNext && !IsPlaying())
            // {
            //     // TODO: SoundModule hookup.
            //     // ((VolumeCalculator) _calculators[_volumeProperty]).PrepareToPlay(playFlags);   
            // }
            //
            // // Remove the "play next" flag - because it's play next just for this sound, not for the child.
            // playFlags &= ~PlaySoundFlags.PlayNext;
            //
            // if (scheduledStart)
            // {
            //     child.PlayScheduled(startTime, playFlags);
            // }
            // else
            // {
            //     child.Play(playFlags);
            // }
            //
            // SetStatus(SoundStatus.Paused, false);
            //
            // // TODO: This should go higher.
            // // if (_soundDefinition.Throttle)
            // // {
            // //     _soundDefinition.OnPlayed();
            // // }
        }
        #endregion
        
        #region Sound Method Overrides
        
        // NOTE: Moved to ContainerSound
        // protected override bool CanPlay(PlaySoundFlags playSoundFlags = PlaySoundFlags.None)
        // {
        //     if (!base.CanPlay(playSoundFlags)) return false;
        //     
        //     // TODO: The throttle bit should be in the StandardSound class.
        //     // if (_soundDefinition.Throttle && !_soundDefinition.CanPlay(playSoundFlags))
        //     // {
        //     //     return false;
        //     // }
        //     
        //     // This is the first time we're playing this sound. Let's INIT the looping situation.
        //     if (!HasPlayFlag(playSoundFlags, PlaySoundFlags.PlayNext))
        //     {
        //         InitPlaybackValues();
        //     }
        //
        //     if (HasType(SoundType.Continuous) && _scheduleChildren && !_playMore) return false;
        //     
        //     if (_child != null)
        //     {
        //         if (!_child.IsValid())
        //         {
        //             Debug.LogWarning($"HEAR XR: Unable to play {this} because child is invalid.");
        //             return false;
        //         }
        //
        //         return true;
        //     }
        //
        //     if (PrepareChild() && _child.IsValid())
        //     {
        //         return true;
        //     }
        //
        //     Debug.LogWarning($"HEAR XR: Unable to play {this} because unable to prepare child.");
        //     return false;
        // }
        
        // protected override void PostSetSoundSourceObject()
        // {
        //     base.PostSetSoundSourceObject();
        //     if (_child != null)
        //     {
        //         _child.SoundSourceObject = SoundSourceObject;
        //     }
        // }

        protected override void PostPlaybackEndedGetReadyToPlayAgain()
        {
            base.PostPlaybackEndedGetReadyToPlayAgain();
            
            // TODO: Sort this.
            // InitPlaybackValues();
        }
        
        // TODO: SoundModule hookup.
        // protected override void GatherSoundPropertyInfo(ref Dictionary<SoundProperty, Definition> soundPropertyInfo)
        // {
        //     base.GatherSoundPropertyInfo(ref soundPropertyInfo);
        //     soundPropertyInfo.Add(_repeatsProperty, _soundDefinition.repeatsDefinition);
        //     soundPropertyInfo.Add(_timeBetweenProperty, _soundDefinition.timeBetweenDefinition);
        // }
        #endregion
        
        #region ParentSound Overrides
        protected override void DoOnChildBeforeEnded(ISound child, double endTime, int nonStoppedChildrenLeft)
        {
            base.DoOnChildBeforeEnded(child, endTime, nonStoppedChildrenLeft);
            if (_playMore)
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
        // NOTE: Moved to CoreSchedulerSoundProcessor
        // private void InitPlaybackValues()
        // {
        //     if (!IsContinuous()) return;
        //     _childIndex = -1;
        //
        //     // Special case - this is a continuous sound with only one child, and no breaks between children.
        //     // In that case tell the child to loop itself.
        //     CheckSingleLoopingChild();
        // }
        
        // NOTE: Moved to CoreSchedulerSoundProcessor
        // private void CheckSingleLoopingChild()
        // {
        //     // TODO: SoundModule hookup.
        //     if (HasSingleLoopingChild())
        //     {
        //         // TODO: Maybe this logic can / should live inside the sound definition?
        //         // TODO: I don't like this. It's uniquely specific to AudioClips so we don't have to schedule when we can just loop.
        //         _soundDefinition.Children[0].loop = true;
        //         _soundDefinition.Children[0].loopCount = _calculators[_repeatsProperty].ValueContainer.IntValue;
        //     }
        //     else
        //     {
        //         _repeatsRemaining = _calculators[_repeatsProperty].ValueContainer.IntValue; 
        //         _limitedRepeats = (_repeatsRemaining > 0);
        //         _playMore = true;
        //         // Debug.Log($"{this} Setting playMore to TRUE");
        //         _scheduleChildren = true;
        //     }
        // }
        
        protected virtual bool HasSingleLoopingChild()
        {
            return false;
            // return (IsContinuous() && 
            //         _soundDefinition.ChildCount == 1 &&
            //         _calculators[_timeBetweenProperty].ValueContainer.FloatValue <= Mathf.Epsilon &&
            //         !_soundDefinition.timeBetweenDefinition.randomize);
        }
        
        private bool PrepareChild()
        {
            if (_child != default)
            {
                Debug.Log($"HEAR XR: {this} already has a prepared child.");
                return true;
            }

            _childIndex = _soundDefinition.GetNextChildIndex(_childIndex);
            _child = (ISoundInternal) _children[_childIndex];

            if (_child == default || !_child.IsValid())
            {
                Debug.LogError($"HEAR XR: We don't have the next child.");
                return false;
            }

            _child.PrepareToPlay();

            return true;
        }

        protected void SetRepeats(int newRepeats)
        {
            _repeatsRemaining = newRepeats;
            if (_repeatsRemaining < 0)
            {
                _limitedRepeats = false;
                _playMore = true;
                // Debug.Log($"{this} Setting Play More to TRUE TRUE");
            }
            else
            {
                _limitedRepeats = true;
                _playMore = _repeatsRemaining > 0;
                // if (_playMore)
                // {
                //     Debug.Log($"{this} Setting Play More to TRUE TRUE TRUE");
                // }
                // else
                // {
                //     Debug.Log($"{this} Setting Play More to FALSE FALSE FALSE");
                // }
            }
            // Debug.Log($"{this} new repeats {_repeatsRemaining} play more? {_playMore} limited repeats? {_limitedRepeats}");
        }
        #endregion
        
        private void SetPlaybackSettings()
        {
            _childIndex = -1;
            _limitedRepeats = false;
            _playMore = false;
            // Debug.Log($"{this} Setting playMore to FALSE");
            _scheduleChildren = false;
        
            switch (_soundDefinition.parentSoundType)
            {
                case ParentSoundType.Continuous:
                    SetType(SoundType.Continuous, true);
                    
                    // TODO: Sort this.
                    // CheckSingleLoopingChild();
                    break;
                
                case ParentSoundType.OneShot:
                    SetType(SoundType.OneShot, true);
                    break;
                    
                default:
                    Debug.LogError("HEAR XR: Unknown sound type");
                    break;
            }
            
            _playbackOrder = _soundDefinition.playbackOrder;
            _repeatType = _soundDefinition.repeatType;
        }
        
        protected virtual void DecrementRepeatsRemaining(int childIndexPlayed)
        {
            if (!_limitedRepeats) return;
            
            if (_repeatsRemaining <= 0) return;
        
            if (_repeatType == ParentSoundRepeatType.Child)
            {
                --_repeatsRemaining;
            }
            else if (_repeatType == ParentSoundRepeatType.Self &&
                     childIndexPlayed >= _soundDefinition.ChildCount - 1)
            {
                // TODO: This seems like it might cause a bug with Random containers?
                --_repeatsRemaining;
            }
        
            // Debug.Log($"{this} new repeats remaining: {_repeatsRemaining}");
            
            if (_repeatsRemaining <= 0)
            {
                // Debug.Log($"{this} Setting playMore to FALSE");
                _playMore = false;
            }
            
            // Debug.Log($"{this} DECREMENTED repeats {_repeatsRemaining} play more? {_playMore} limited repeats? {_limitedRepeats}");
        }
        
        protected virtual double GetScheduledStartTime()
        {
            return AudioSettings.dspTime;
        }
    }
}