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
        // protected override void PostParseSoundDefinition()
        // {
        //     base.PostParseSoundDefinition();
        //     Debug.Log($"Post parse {this} sound definition");
        // }

        protected override void ApplySoundPropertyValues(SetValuesType setValuesType)
        {
            base.ApplySoundPropertyValues(setValuesType);
            
            if (!IsValid()) return;
        
            // TODO: SoundModule hookup.
            // var properties = _soundPropertiesBySetType[setValuesType];
            //
            // for (int i = 0; i < properties.Length; ++i)
            // {
            //     if (!_calculators.ContainsKey(properties[i]))
            //     {
            //         Debug.LogError($"HEAR XR {this} doesn't have the calculator for {properties[i].name}");
            //     }
            //
            //     _calculators[properties[i]].Calculate();
            //     var value = _calculators[properties[i]].ValueContainer.FloatValue;
            //
            //     if (properties[i] == _volumeProperty)
            //     {
            //         _audioSource.volume = value;
            //     }
            //     else if (properties[i] == _pitchProperty)
            //     {
            //         _audioSource.pitch = value;
            //     }
            //     else if (properties[i] == _delayProperty)
            //     {
            //         continue; // Delay is used at the moment of playing.
            //     }
            //     else if (properties[i] == _offsetProperty)
            //     {
            //         var audioClip = _soundDefinition.AudioClip;
            //         _audioSource.timeSamples = TimeSamplesHelper.ValidateAudioClipOffset(in audioClip, value);
            //     }
            //     else
            //     {
            //         Debug.LogWarning($"HEAR XR: {this} Unable to apply property {properties[i].name}");
            //     }
            // }
        }

        protected event Action<AudiobreadSource> ApplySoundDefinitionToUnityAudio;
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

        protected override void ApplySoundDefinitionAndProperties(InitSoundFlags initSoundFlags = InitSoundFlags.None)
        {
            base.ApplySoundDefinitionAndProperties(initSoundFlags);
            ApplySoundDefinitionToUnityAudio?.Invoke(_audiobreadSource);
            // foreach (var smProcessor in _soundModuleProcessors)
            // {
            //     // TODO:
            // }
        }
        
        protected override void DoPlay(PlaySoundFlags playFlags, bool scheduled, double startTime = -1.0d)
        {
            // If asked to play when already playing, reset loop counts.
            if (IsPlayingOrTransitioning())
            {
                ResetLoopCounts();
            }
            
            // Unset ReadyToPlay because we can only play one thing at a time.
            SetStatus(SoundStatus.ReadyToPlay, false);

            // TODO: SORT OUT THE DELAY THINGY
            // If there is any delay on the sound, we should always just use PlayScheduled.
            // var delay = _calculators[_delayProperty].ValueContainer.FloatValue;
            // if (delay > 0.0f)
            // {
            //     scheduled = true;
            // }

            var delay = 0.0f;
            // scheduled = false;

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
                _audioSource.Play();
                SetStatus(SoundStatus.Paused, false);
                SetPlaybackState(PlaybackState.Playing);
                InvokeOnBegan(this, AudioSettings.dspTime);
                return;
            }

            DoPlayScheduled(startTime, delay);
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