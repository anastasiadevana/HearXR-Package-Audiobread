using System;
using System.Collections.Generic;
using HearXR.Audiobread.SoundProperties;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HearXR.Audiobread
{
    public class ToneGenerator : SoundGeneratorUnityAudio<ToneGeneratorDefinition, ToneGenerator>
    {
        #region Private Fields
        // Tone generator specific fields.
        private float _toneDuration = 2.0f; // TODO: Put in definition.
        private int _clipChannels = 2; // TODO: Should we do 1 channel???
        private float _toneFrequency;
        private WaveShape _waveShape;
        private AudioClip _audioClip;
        private int _clipPosition;
        private float _squareVolumeFactor = 0.4f; // TODO: Definition
        private float _sawtoothVolumeFactor = 0.4f; // TODO: Put into definition
        private float _sineVolumeFactor = 1.0f; // TODO: Put into definition
        #endregion
        
        #region Constructor
        public ToneGenerator(AudiobreadSource audiobreadSource) : base(audiobreadSource) {}
        #endregion
        
        #region Sound Abstract Methods
        protected override void DoPlay(PlaySoundFlags playFlags, bool scheduled, double startTime = -1.0d)
        {
            // Always treat as scheduled.
            base.DoPlay(playFlags, true, startTime);
        }
        
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
            //     // else if (properties[i] == _offsetProperty)
            //     // {
            //     //     // TODO: Generate a little clip info that will provide clip frequency, length, etc.
            //     //     // var audioClip = _soundDefinition.AudioClip;
            //     //     // _audioSource.timeSamples = TimeSamplesHelper.ValidateAudioClipOffset(in audioClip, value);
            //     // }
            //     else
            //     {
            //         Debug.LogWarning($"HEAR XR: {this} Unable to apply property {properties[i].name}");
            //     }
            // }
        }
        
        protected override void DoReleaseResources()
        {
            _inUse = false;
            DestroyAudioClip();
            ((ISoundInternal) this).DeInit();
            ResetToDefaults();
        }
        #endregion

        #region SoundGeneratorUnityAudio Abstract Methods
        protected override void SetUpAudioSource()
        {
            // TODO: NO MAGIC NUMBERS
            _clipSampleRate = 44100;
            
            _toneFrequency = _soundDefinition.Frequency;

            // Save some sample and frequency information about this clip for further calculations.
            _clipOneSampleDuration = TimeSamplesHelper.GetSingleSampleDuration(_clipSampleRate);

            _clipTotalSamples = TimeSamplesHelper.TimeToSamples(_toneDuration * 1.0d, _clipSampleRate);
            _beforeCompletedSamplesThreshold = TimeSamplesHelper.TimeToSamples(SCHEDULING_BUFFER, _clipSampleRate);

            _audiobreadSource.Mode = AudiobreadSource.AudioSourceMode.ToneGenerator;
            CreateAudioClip();
            _audioSource.clip = _audioClip;
            _audioSource.loop = true;
        }
        
        protected override void ResetToDefaults()
        {
            base.ResetToDefaults();
            _clipSampleRate = AudioSettings.outputSampleRate;
        }
        #endregion

        #region Private Methods
        // TODO: Do I actually need to destroy and create?
        private void DestroyAudioClip()
        {
            if (_audioClip == null) return;
            if (_audioSource != null)
            {
                _audioSource.clip = null;
            }
            Object.Destroy(_audioClip);
            _audioClip = null;
        }

        private void CreateAudioClip()
        {
            if (_audioClip != null)
            {
                // TODO: CAN this ever happen???
                Debug.LogWarning("ToneGenerator: Already have an AudioClip... How did that happen?");
            }
            else
            {
                Debug.Log("ToneGenerator: Create audio clip");
                _audioClip = AudioClip.Create(_soundDefinition.Name, _clipTotalSamples, _clipChannels, _clipSampleRate, true, OnAudioClipRead, OnAudioClipSetPosition);   
            }
        }
        
        private void OnAudioClipRead(float[] buffer)
        {
            // Using direct generation methods.
            for (var i = 0; i < buffer.Length; i += _clipChannels)
            {
                switch (_waveShape)
                {
                    case WaveShape.Sin:
                        //buffer[i] = CreateSine(_timeIndex, _frequency, _sampleRate);
                        // TODO: Try to use complex number so that we don't have to run Sin all the time.
                        buffer[i] = Mathf.Sin(Mathf.PI * 2.0f * _clipPosition * _toneFrequency / _clipSampleRate) * _sineVolumeFactor;
                        break;
                
                    case WaveShape.Square:
                        buffer[i] = ((Mathf.Repeat(_clipPosition * _toneFrequency / _clipSampleRate,1) > 0.5f) ? 1.0f : -1.0f) * _squareVolumeFactor;
                        break;
                
                    case WaveShape.Sawtooth:
                        buffer[i] = (Mathf.Repeat(_clipPosition * _toneFrequency / _clipSampleRate,1) * 2.0f - 1.0f) * _sawtoothVolumeFactor;
                        break;
                
                    case WaveShape.Triangle:
                        buffer[i] = Mathf.PingPong(_clipPosition * 2.0f * _toneFrequency / _clipSampleRate,1) * 2.0f - 1.0f;
                        break;
                
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                for (var j = 1; j < _clipChannels; ++j)
                {
                    buffer[i + j] = buffer[i];
                }
            
                _clipPosition++;
        
                // TODO: No magic numbers
                // TODO: Do we need this? Maybe we shouldn't reset clip position, unless it's looping or something...
                // If timeIndex gets too big, reset it to 0
                // if (_timeIndex >= _clipSampleRate * 2)
                // {
                //     _timeIndex = 0;
                // }
            }
        }

        private void OnAudioClipSetPosition(int newPosition)
        {
            _clipPosition = newPosition;
        }
        #endregion
        
        #region Helper Methods
        public override string ToString()
        {
            return $"- TONE GENERATOR - [{Guid}] [{_soundDefinition.Frequency}]";
        }
        #endregion
    }
}