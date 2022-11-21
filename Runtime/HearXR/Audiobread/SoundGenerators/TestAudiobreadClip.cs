using System;
using System.Collections.Generic;
using HearXR.Audiobread.SoundProperties;
using UnityEngine;

namespace HearXR.Audiobread
{
    public class TestAudiobreadClip : SoundGeneratorUnityAudio<TestSoundDefinition, TestAudiobreadClip>
    {
        #region Constructor
        public TestAudiobreadClip(AudiobreadSource audiobreadSource) : base(audiobreadSource) {}
        #endregion

        #region Sound Abstract Methods
        protected override void DoPlay(PlaySoundFlags playFlags)
        {
            // If asked to play when already playing, reset loop counts.
            if (IsPlayingOrTransitioning())
            {
                ResetLoopCounts();
            }
            
            // Unset ReadyToPlay because we can only play one thing at a time.
            SetStatus(SoundStatus.ReadyToPlay, false);

            // Stop the sound that was paused.
            // TODO: Do we care about "scheduledStart" here? Should we just always stop paused sources?
            if (IsPaused() && _instancePlaybackInfo.scheduledStart)
            {
                _audioSource.Stop();
            }

            SetSchedulableState(SchedulableSoundState.None);

            // If PlayScheduled() was not requested, and there are no delays, just play it and get it over with.
            if (!_instancePlaybackInfo.scheduledStart)
            {
                _audioSource.Play();
                SetStatus(SoundStatus.Paused, false);
                SetPlaybackState(PlaybackState.Playing);
                InvokeOnBegan(this, AudioSettings.dspTime);
                return;
            }

            DoPlayScheduled();
        }
        
        protected override void DoReleaseResources()
        {
            _inUse = false;
            ((ISoundInternal) this).DeInit();
            ResetToDefaults();
        }
        #endregion

        #region SoundGeneratorUnityAudio Abstract Methods
        protected override void SetUpAudioSource()
        {
            _audioSource.clip = _soundDefinition.AudioClip;
            // _audioSource.loop = true;

            // Save some sample and frequency information about this clip for further calculations.
            _clipSampleRate = _soundDefinition.AudioClip.frequency;
            _clipOneSampleDuration = TimeSamplesHelper.GetSingleSampleDuration(_clipSampleRate);
            _clipTotalSamples = _soundDefinition.AudioClip.samples;
            _beforeCompletedSamplesThreshold = TimeSamplesHelper.TimeToSamples(SCHEDULING_BUFFER, _clipSampleRate);
            
            _audiobreadSource.Mode = AudiobreadSource.AudioSourceMode.ClipPlayer;
        }
        #endregion

        #region Helper Methods
        public override string ToString()
        {
            return _soundDefinition != null 
                ? $"- TEST AUDIO CLIP - [{Guid}] [{_soundDefinition.AudioClip.name}]" 
                : $"- TEST AUDIO CLIP - [{Guid}]";
        }
        #endregion

        // protected override bool CanStop(StopSoundFlags stopSoundFlags = StopSoundFlags.None)
        // {
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
        // }
    }
}