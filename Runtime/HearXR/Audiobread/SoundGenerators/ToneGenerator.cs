using System;
using UnityEngine;

namespace HearXR.Audiobread
{
    public class ToneGenerator : SoundGeneratorUnityAudio<ToneGeneratorDefinition, ToneGenerator>, IToneGeneratorSound
    {
        #region Private Fields
        // TODO: If the duration is longer than the clip length, there's a bad loop point. Figure out how to fix this.
        // TODO: Maybe we need to switch to OnFilterRead instead.
        private float _toneDuration = 10.0f;
        private int _clipChannels = 1;
        private AudioClip _audioClip;
        private int _clipPosition;

        private ToneGeneratorSettings _settings;
        private bool _initSettings;
        
        // private WaveShapeEnum _waveShape;
        
        // private float _toneFrequency;
        
        // TODO: Should these be in here?
        private float _squareVolumeFactor = 0.4f;
        private float _sawtoothVolumeFactor = 0.4f;
        private float _sineVolumeFactor = 1.0f;
        
        private bool _audioClipCreated;
        private bool _audioClipAssigned;
        #endregion
        
        #region Constructor
        public ToneGenerator(AudiobreadSource audiobreadSource) : base(audiobreadSource) {}
        #endregion
        
        // TODO: Slide to new tone frequency instead of jumping.
        #region IToneGeneratorSound
        public ToneGeneratorSettings Settings
        {
            get
            {
                if (!_initSettings)
                {
                    _settings = new ToneGeneratorSettings();
                    _initSettings = true;
                }
                return _settings;
            }
            set
            {
                _settings = value;
                CreateAudioClip();
            }
        }
        #endregion

        #region Sound Abstract Methods
        protected override void DoPlay(PlaySoundFlags playFlags)
        {
            // Always treat as scheduledStart.
            _instancePlaybackInfo.scheduledStart = true;

            base.DoPlay(playFlags);
        }

        protected override void DoReleaseResources()
        {
            _inUse = false;
            UnassignAudioClip();
            ((ISoundInternal) this).DeInit();
            ResetToDefaults();
        }
        #endregion

        #region SoundGeneratorUnityAudio Abstract Methods
        protected override void SetUpAudioSource()
        {
            // TODO: NO MAGIC NUMBERS
            _clipSampleRate = 44100;
            
            //_toneFrequency = _soundDefinition.Frequency.FloatValue;
            // _toneFrequency = 440.0f;

            // Save some sample and frequency information about this clip for further calculations.
            _clipOneSampleDuration = TimeSamplesHelper.GetSingleSampleDuration(_clipSampleRate);

            _clipTotalSamples = TimeSamplesHelper.TimeToSamples(_toneDuration * 1.0d, _clipSampleRate);
            _beforeCompletedSamplesThreshold = TimeSamplesHelper.TimeToSamples(SCHEDULING_BUFFER, _clipSampleRate);

            _audiobreadSource.Mode = AudiobreadSource.AudioSourceMode.ToneGenerator;
            
            _audioSource.loop = true;
        }
        
        protected override void ResetToDefaults()
        {
            base.ResetToDefaults();
            _clipSampleRate = AudioSettings.outputSampleRate;
        }
        #endregion
        
        
        #region Private Methods
        private void UnassignAudioClip()
        {
            if (_audioClip == null) return;
            if (_audioSource != null)
            {
                _audioSource.clip = null;
                _audioClipAssigned = false;
            }
        }
        
        private void CreateAudioClip()
        {
            if (!_audioClipCreated)
            {
                if (_audioClip != null)
                {
                    Debug.LogWarning("ToneGenerator: Already have an AudioClip... How did that happen?");
                }
                else
                {
                    _audioClip = AudioClip.Create(_soundDefinition.Name, _clipTotalSamples, _clipChannels, _clipSampleRate, true, OnAudioClipRead, OnAudioClipSetPosition);   
                }
                _audioClipCreated = true;   
            }

            AssignAudioClip();
        }

        private void AssignAudioClip()
        {
            _audioSource.clip = _audioClip;
            _audioClipAssigned = true;
        }
        
        // TODO: Instead of using an audio clip, try to use OnAudioFilterRead. The preload on the audio clip is causing some weirdness.
        private void OnAudioClipRead(float[] buffer)
        {
            // Using direct generation methods.
            for (var i = 0; i < buffer.Length; i += _clipChannels)
            {
                switch (_settings.waveShape)
                {
                    case WaveShapeEnum.Sin:
                        //buffer[i] = CreateSine(_timeIndex, _frequency, _sampleRate);
                        // TODO: Try to use complex number so that we don't have to run Sin all the time.
                        buffer[i] = Mathf.Sin(Mathf.PI * 2.0f * _clipPosition * _settings.frequency / _clipSampleRate) * _sineVolumeFactor;
                        break;
                
                    case WaveShapeEnum.Square:
                        buffer[i] = ((Mathf.Repeat(_clipPosition * _settings.frequency / _clipSampleRate,1) > 0.5f) ? 1.0f : -1.0f) * _squareVolumeFactor;
                        break;
                
                    case WaveShapeEnum.Sawtooth:
                        buffer[i] = (Mathf.Repeat(_clipPosition * _settings.frequency / _clipSampleRate,1) * 2.0f - 1.0f) * _sawtoothVolumeFactor;
                        break;
                
                    case WaveShapeEnum.Triangle:
                        buffer[i] = Mathf.PingPong(_clipPosition * 2.0f * _settings.frequency / _clipSampleRate,1) * 2.0f - 1.0f;
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
            return $"- TONE GENERATOR - [{Guid}]";
        }
        #endregion
    }
}