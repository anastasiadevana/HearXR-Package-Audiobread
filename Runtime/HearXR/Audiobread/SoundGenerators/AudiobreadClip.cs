using System.Collections.Generic;
using HearXR.Audiobread.SoundProperties;
using UnityEngine;

namespace HearXR.Audiobread
{
    public class AudiobreadClip : SoundGeneratorUnityAudio<AudiobreadClipDefinition, AudiobreadClip>
    {
        #region Constructor
        public AudiobreadClip(AudiobreadSource audiobreadSource) : base(audiobreadSource) {}
        #endregion

        #region Sound Abstract Methods
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
            return $"- AUDIO CLIP - [{Guid}] [{_soundDefinition.AudioClip.name}]";
        }
        #endregion
    }
}
