using System;
using UnityEngine;

namespace HearXR.Audiobread
{
    public abstract class SoundGeneratorUnityAudio<TDefinition, TSelf> : Sound<TDefinition, TSelf>, ISoundGeneratorUnityAudio
        where TDefinition : class, ISoundDefinition
        where TSelf : Sound
    {
        #region Events
        public event Action<ISoundGeneratorUnityAudio, double> OnStealRequested;
        protected event Action<AudiobreadSource> ApplySoundDefinitionToUnityAudio;
        #endregion
        
        #region Properties
        internal AudiobreadSource Source => _audiobreadSource;
        #endregion
        
        #region Protected Fields
        protected AudiobreadSource _audiobreadSource;
        protected AudioSource _audioSource;
        protected bool _inUse;
        
        // Clip frequency-dependent numbers
        protected int _clipSampleRate; // TODO: No magic numbers.
        protected double _clipOneSampleDuration;
        protected int _clipTotalSamples;
        protected int _beforeCompletedSamplesThreshold;
        
        // Looping
        protected bool _looping;
        protected bool _countingLoops;
        protected int _loopsCounted;
        protected int _numberOfLoopsToPlay;
        protected int _previousClipSample;
        
        // TODO: Probably don't need to store all of these.
        // TODO: Reset all these values when "rinsing".
        // Support values for scheduling.
        protected double _clipStartTime;
        protected double _actualClipStartTime;
        protected int _clipStartSamples;
        protected int _clipPlayingSamples;
        protected double _clipPlayingLength;
        protected double _clipEndTime;
        
        // Pause / resume support.
        protected double _scheduledTimeRemainingOnPause;
        #endregion
        
        #region Private Fields
        private bool _2DSound = false; // TODO: This is just hardcoded???
        #endregion
        
        #region Properties
        public AudiobreadSource AudiobreadSource => _audiobreadSource;
        #endregion

        #region Constructor
        protected SoundGeneratorUnityAudio(AudiobreadSource audiobreadSource)
        {
            if (audiobreadSource == null || audiobreadSource.AudioSource == null)
            {
                Debug.LogError($"HEAR XR: {this} AudiobreadSource is null.");
                ResetStatus(SoundStatus.Invalid);
                return;
            }
            
            _audiobreadSource = audiobreadSource;
            _audioSource = _audiobreadSource.AudioSource;
            
            ResetToDefaults();
        }
        #endregion
        
        #region Sound Abstract Methods
        protected override void DoPlay(PlaySoundFlags playFlags, bool scheduled, double startTime = Audiobread.INACTIVE_START_TIME)
        {
            // If asked to play when already playing, reset loop counts.
            if (IsPlayingOrTransitioning())
            {
                ResetLoopCounts();
            }
            
            // Unset ReadyToPlay because we can only play one thing at a time.
            SetStatus(SoundStatus.ReadyToPlay, false);
            
            // TODO: SoundModule hookup. (delay)
            // If there is any delay on the sound, we should always just use PlayScheduled.
            // var delay = _calculators[_delayProperty].ValueContainer.FloatValue;
            // if (delay > 0.0f)
            // {
            //     scheduled = true;
            // }

            // Stop the sound that was paused.
            // TODO: Do we care about "scheduled" here? Should we just always stop paused sources?
            if (IsPaused() && scheduled)
            {
                _audioSource.Stop();
            }

            SetSchedulableState(SchedulableSoundState.None);

            // If PlayScheduled() was not requested, and there are no delays, just play it and get it over with.
            if (!scheduled)
            {
                Debug.Log("Not scheduled!");
                
                _audioSource.Play();
                SetStatus(SoundStatus.Paused, false);
                SetPlaybackState(PlaybackState.Playing);
                InvokeOnBegan(this, AudioSettings.dspTime);
                return;
            }
            
            DoPlayScheduled(startTime);
        }
        
        protected void DoPlayScheduled(double startTime)
        {
            if (startTime < 0.0d)
            {
                startTime = AudioSettings.dspTime;
            }

            _clipStartTime = startTime;

            // TODO: Make sure that _clipStartTime is never in the past (sometimes it is).
            // Make sure that the play scheduled time is never negative.
            // if (_clipStartTime < AudioSettings.dspTime)
            // {
            //     _clipStartTime = AudioSettings.dspTime;
            // }
            _audioSource.PlayScheduled(_clipStartTime);
            SetStatus(SoundStatus.Paused, false);

            OnDidScheduleSound();

            SetSchedulableState(SchedulableSoundState.Scheduled);
        }
        
        protected override void DoStop(StopSoundFlags flags)
        {
            SetSchedulableState(SchedulableSoundState.None);
            _audioSource.Stop();
            CheckIfEnded();
        }
        
        protected override void DoPause()
        {
            if (PlaybackState == PlaybackState.PlayInitiated)
            {
                // Save how much time was left on the scheduled sound, so that we can resume after unpause.
                _scheduledTimeRemainingOnPause = _clipStartTime - AudioSettings.dspTime;
                if (_scheduledTimeRemainingOnPause < 0.0d) _scheduledTimeRemainingOnPause = 0.0d;
                _audioSource.Stop();
                return;
            }

            _audioSource.Pause();
        }
        
        protected override void DoResume()
        {
            if (PlaybackState == PlaybackState.PlayInitiated)
            {
                var startTime = AudioSettings.dspTime + _scheduledTimeRemainingOnPause;
                DoPlayScheduled(startTime);
                return;
            }

            _audioSource.UnPause();
        }
        
        protected override void DoMute()
        {
            _audioSource.mute = true;
        }

        protected override void DoUnmute()
        {
            _audioSource.mute = false;
        }
        
        protected override void PostSetSoundDefinition(InitSoundFlags initSoundFlags = InitSoundFlags.None)
        {
            base.PostSetSoundDefinition(initSoundFlags);
            _inUse = true;
        }

        protected override void PostSetSoundSourceObject()
        {
            base.PostSetSoundSourceObject();
            _audiobreadSource.ObjectToFollow = (_soundSourceObject == null) ? null : _soundSourceObject.transform;
            UpdateFollowingState();
        }
        
        protected override void PostPlaybackStateChanged(PlaybackState previousState, PlaybackState newState)
        {
            UpdateFollowingState();
            if (newState == PlaybackState.Stopped && _inUse)
            {
                ResetLoopCounts();
            }
        }
        
        protected override void PostInitModules(InitSoundFlags initSoundFlags = InitSoundFlags.None)
        {
            base.PostInitModules(initSoundFlags);
            
            // TODO: This should be in the parent UnityAudio class.
            // TODO: When do we unsubscribe from this?
            ApplySoundDefinitionToUnityAudio += _soundModuleGroupProcessor.HandleApplySoundDefinitionToUnityAudio;
            // foreach (var smProcessor in _soundModuleProcessors)
            // {
            //     ApplySoundDefinitionToUnityAudio += smProcessor.ApplySoundDefinitionToUnityAudio;
            // }
        }

        // protected override void PostSetParent(ISound previousParent, ISound newParent)
        // {
        //     base.PostSetParent(previousParent, newParent);
        //     if (newParent == null || newParent == previousParent) return; // TODO: Not sure how this would happen or what to do about this.
        //     
        //     // TODO: Do we need to do this with multiple nested sounds? Maybe it needs to go somewhere higher.
        //     if (newParent.GetCalculators(out Dictionary<SoundProperty, Calculator> parentCalculators))
        //     {
        //         if (_soundProperties == null)
        //         {
        //             Debug.LogError("There are no sound properties!");
        //         }
        //         for (int i = 0; i < _soundProperties.Length; ++i)
        //         {
        //             if (parentCalculators.ContainsKey(_soundProperties[i]))
        //             {
        //                 _calculators[_soundProperties[i]].AddInfluence(parentCalculators[_soundProperties[i]].ValueContainer);
        //             }
        //         }
        //     }
        // }
        
        protected override void ApplySoundDefinitionAndProperties(InitSoundFlags initSoundFlags = InitSoundFlags.None)
        {
            base.ApplySoundDefinitionAndProperties(initSoundFlags);

            SetUpAudioSource();

            //Debug.Log($"HEAR XR: {this} applying sound definition and properties");
            SetLooping();

            ApplySoundDefinitionToUnityAudio?.Invoke(_audiobreadSource);
            // foreach (var smProcessor in _soundModuleProcessors)
            // {
            //     // TODO:
            // }
            
            // TODO: Set spatialization
            // SetSpatializationType(_parentSound.SoundDefinition.SpatializationType);
            
            // TODO: Set Mixer Group.
        }

        protected override bool CanPrepareToPlay()
        {
            if (!base.CanPrepareToPlay()) return false;
            // If we're already playing, we cannot also be ready to play.
            return IsStopped();
        }
        
        protected override bool ShouldUpdate()
        {
            return base.ShouldUpdate() && !IsStoppedOrPaused();
        }

        protected override bool CanMute()
        {
            return base.CanMute() || !_audioSource.mute;
        }

        protected override bool CanUnmute()
        {
            return base.CanUnmute() || _audioSource.mute;
        }

        internal override void DoUpdate()
        {
            base.DoUpdate();
            
            CheckLoopCounts();
            
            CheckIfEnded();

            switch (_schedulableState)
            {
                case SchedulableSoundState.Scheduled:
                    CheckIfScheduledBegan();
                    break;

                case SchedulableSoundState.Playing:
                    CheckIfBeforeEnded();
                    break;

                case SchedulableSoundState.Completing:
                    CheckIfEnded();
                    break;
            }

            // The sound might be released after this point. So no reason to continue updating.
            if (!_inUse) return;
            
#if UNITY_EDITOR
            if (_soundDefinition.WasChanged)
            {
                InvokeSoundDefinitionChanged();
                // NOTE: Moved to SoundModuleProcessor
                // RefreshCalculators();
                // END
            }
#endif
            InvokeUnityAudioGeneratorTickEvent();
            // ApplySoundPropertyValues(SetValuesType.OnUpdate);
        }
        #endregion
        
        #region Protected Methods
        protected void SetSchedulableState(SchedulableSoundState newState) 
        {
            if (newState == _schedulableState) return;

            _schedulableState = newState;
            switch (newState)
            {
                case SchedulableSoundState.Scheduled:
                    SetPlaybackState(PlaybackState.PlayInitiated);
                    InvokeOnScheduled(this, _clipStartTime);
                    CheckIfScheduledBegan();
                    break;

                case SchedulableSoundState.Playing:
                    SetPlaybackState(PlaybackState.Playing);
                    
                    Debug.Log("Schedulable is playing now!");
                    
                    InvokeOnBegan(this, _actualClipStartTime);
                    _actualClipStartTime = -1;
                    CheckIfBeforeEnded();
                    break;

                case SchedulableSoundState.Completing:
                    InvokeOnBeforeEnded(this, GetProjectedGeneratorEndTime());
                    CheckIfEnded();
                    break;

                case SchedulableSoundState.None:
                    // Nothing to do here.
                    break;

                default:
                    Debug.LogError($"HEAR XR: {this} Unknown schedulable state passed in.");
                    break;
            }
        }
        #endregion
        
        #region Private Methods
        private void UpdateFollowingState()
        {
            _audiobreadSource.Follow = (PlaybackState != PlaybackState.Stopped) && !_2DSound;
        }
        #endregion
        
        #region Internal Methods
        internal void PrepareToBeStolen()
        {
            double projectedEndTime = AudioSettings.dspTime;

            // Calculate projected end time based on state.
            if (IsPlayingOrTransitioning())
            {
                projectedEndTime = GetProjectedGeneratorEndTime();
                _audioSource.Stop();
            }

            OnStealRequested?.Invoke(this, projectedEndTime);
            _inUse = false;
        }
        #endregion
        
        #region Overrideable Methods
        protected virtual void SetLooping()
        {
            // TODO: Make looping a sound property.
            // _looping = _soundDefinition.loop && _soundDefinition.loopCount != 1;
            // _numberOfLoopsToPlay = _soundDefinition.loopCount;
            ResetLoopCounts();
        }
        
        protected virtual double GetProjectedGeneratorEndTime()
        {
            return _clipEndTime;
        }
        
        protected virtual void CheckLoopCounts()
        {
            if (!_countingLoops) return;

            // TODO: Bring this back after figuring out how to handle pitch.
            // Can do some optimization here instead of checking every frame,
            // check X seconds, as long as the clip is not shorter than that.
            //bool canWaitLonger = _audioSource.clip.length > 1.0f;

            if (_loopsCounted < _numberOfLoopsToPlay - 1)
            {
                // Wait until the clip starts from the beginning, and increment the loop count.
                if (_audioSource.timeSamples >= _previousClipSample)
                {
                    _previousClipSample = _audioSource.timeSamples;
                    return;
                }

                _previousClipSample = 0;
                _loopsCounted++;
                return;
            }

            // We have come to the last loop. Just uncheck the loop checkbox.
            _audioSource.loop = false;
            _countingLoops = false;
        }
        
        protected virtual void ResetLoopCounts()
        {
            // TODO: Looping - sound property!!!
            // _audioSource.loop = _looping;
            // _countingLoops = _looping && _soundDefinition.loopCount > 1;
            // _loopsCounted = 0;
            // _previousClipSample = 0;
        }

        protected virtual void OnDidScheduleSound()
        {
            // Save some values for status checks.
            _clipStartSamples = _audioSource.timeSamples;
            _clipPlayingSamples = _clipTotalSamples - _clipStartSamples;
            _clipPlayingLength = TimeSamplesHelper.SamplesToTime(_clipPlayingSamples, _clipOneSampleDuration);
            _clipEndTime = _clipStartTime + _clipPlayingLength;
        }
        
        protected virtual void CheckIfScheduledBegan()
        {
            if (!_audioSource.isPlaying) return;

            // There seem to be bugs with very short clips that would report "isPlaying" at the moment of scheduling.
            // That's why we're using a small "safety sample count" to make sure that the audio is actually progressing.
            if (_clipStartSamples + SAFETY_SAMPLE_BUFFER < _audioSource.timeSamples)
            {
                // Calculate the actual clip start time, in case there were any hiccups when scheduling.
                var samplesSinceClipStart = _audioSource.timeSamples - _clipStartSamples;
                var timeSinceClipStart = TimeSamplesHelper.SamplesToTime(samplesSinceClipStart, _clipOneSampleDuration);
                _actualClipStartTime = AudioSettings.dspTime - timeSinceClipStart;
                SetSchedulableState(SchedulableSoundState.Playing);
            }
        }
        
        protected virtual void CheckIfBeforeEnded()
        {
            if (!_audioSource.isPlaying || _audioSource.loop) return;
            
            var samplesRemaining = _clipTotalSamples - _audioSource.timeSamples;
            if (samplesRemaining >= _beforeCompletedSamplesThreshold) return;
            
            // Recalculate the end time, since it's probably more accurate now.
            _clipEndTime = TimeSamplesHelper.SamplesToTime(samplesRemaining, _clipOneSampleDuration) + AudioSettings.dspTime;
            SetSchedulableState(SchedulableSoundState.Completing);
        }
        
        protected virtual void CheckIfEnded()
        {
            if (_audioSource.isPlaying || IsPaused()) return;
            SetPlaybackState(PlaybackState.Stopped);
            SetStatus(SoundStatus.ReadyToPlay, true);
            SetSchedulableState(SchedulableSoundState.None);
            InvokeOnEnded(this);
        }
        
        protected virtual void ResetToDefaults()
        {
            // TODO: Have proper defaults configuration as part of Audiobread settings.
            if (_audiobreadSource == null || _audiobreadSource.AudioSource == null)
            {
                // TODO: can we do anything better here?
                Debug.LogError($"HEAR XR: {this} AudiobreadSource is null.");
                ResetStatus(SoundStatus.Invalid);
                return;
            }
            
            _parentSound = null;
            _soundSourceObject = null;

            _audioSource.clip = default;
            _audioSource.volume = 1.0f;
            _audioSource.pitch = 1.0f;

            _audioSource.loop = false;
            _audioSource.mute = false;
            _audioSource.playOnAwake = false;
            
            _audioSource.spatialize = false;
            _audioSource.spatializePostEffects = false;
            _audioSource.spatialBlend = 0.0f;

            ResetStatus(SoundStatus.None);
            SetPlaybackState(PlaybackState.Stopped);
            SetSchedulableState(SchedulableSoundState.None);

            _countingLoops = false;
            _loopsCounted = 0;
            _numberOfLoopsToPlay = 0;
            _previousClipSample = 0;
        }
        #endregion
        
        #region Abstract Methods
        protected abstract void SetUpAudioSource();
        #endregion
        
        #region Unsorted Methods
        private void SetSpatializationType(SpatializationType spatializationType)
        {
            throw new NotImplementedException();
            // Assign _2dSound here
            //     switch (spatializationType)
            //     {
            //         case SpatializationType.TwoDee:
            //             _audioSource.spatialize = false;
            //             _audioSource.spatializePostEffects = false;
            //             _audioSource.spatialBlend = 0.0f;
            //             break;
            //         case SpatializationType.ThreeDee:
            //             _audioSource.spatialize = false;
            //             _audioSource.spatializePostEffects = false;
            //             _audioSource.spatialBlend = 1.0f;
            //             break;
            //         case SpatializationType.Spatialized:
            //             _audioSource.spatialize = true;
            //             _audioSource.spatializePostEffects = true;
            //             _audioSource.spatialBlend = 1.0f;
            //             break;
            //     }

            //UpdateFollowingState(); // TODO:
        }
        #endregion
    }
}