using System;
using System.Collections;
using HearXR.Audiobread.SoundProperties;
using UnityEngine;

namespace HearXR.Audiobread.Core
{
    public class CoreUnitySoundProcessor : SoundModuleProcessor<CoreUnitySoundModule, CoreUnitySoundModuleDefinition>, IStopControllerProcessor
    {
        private bool _initSoundSource;
        private AudiobreadSource _audiobreadSource;
        private AudioSource _audioSource;

        private VolumeCalculator _volumeCalculator;
        private bool _hasFadeIn;
        // private Fade _fadeIn;
        private bool _hasFadeOut;
        // private Fade _fadeOut;
        
        public CoreUnitySoundProcessor(CoreUnitySoundModule soundModule, ISound sound) : base(soundModule, sound) {}

        protected override void DoApplySoundDefinitionToUnityAudio(AudiobreadSource audiobreadSource)
        {
            base.DoApplySoundDefinitionToUnityAudio(audiobreadSource);
            FindAudioSource(audiobreadSource);
        }

        protected override void PostInitCalculators()
        {
            base.PostInitCalculators();
            
            _hasFadeIn = ModuleSoundDefinition.fadeInDefinition.duration > Mathf.Epsilon;
            // _fadeIn = (_hasFadeIn) ? ModuleSoundDefinition.fadeInDefinition : null;
            _hasFadeOut = ModuleSoundDefinition.fadeOutDefinition.duration > Mathf.Epsilon;
            // _fadeOut = (_hasFadeOut) ? ModuleSoundDefinition.fadeOutDefinition : null;
            
            // Add fades to the volume calculator.
            _volumeCalculator = (VolumeCalculator) _calculators[BuiltInData.Properties.GetSoundPropertyByType<Volume>()];
            _volumeCalculator.SetFades(ModuleSoundDefinition.fadeInDefinition, ModuleSoundDefinition.fadeOutDefinition);
            _volumeCalculator.OnFadeOutFinished += HandleFadeOutFinished;
            _volumeCalculator.OnFadeOutFinished += HandleFadeInFinished;
            _volumeCalculator.PrepareToPlay();
            
            // TODO: Unsubscribe from this at some point?
            // ((VolumeCalculator) _calculators[ModuleSoundDefinition.VolumeProperty]).OnFadeFinished += delegate(Fade.Direction direction)
            // {
            //     // TODO: Handle fade in finished.
            //     if (direction == Fade.Direction.Out)
            //     {
            //         OnFadeOutFinished();
            //     }
            // };
        }
        
        private StopControllerStopCallback _stopCallbackWaiting;

        private void HandleFadeOutFinished()
        {
            _stopCallbackWaiting?.Invoke();
            _stopCallbackWaiting = null;
        }

        private void HandleFadeInFinished()
        {
            
        }

        protected override void OnSoundBegan(ISound sound, double startTime)
        {
            base.OnSoundBegan(sound, startTime);
            
            // TODO: Compare sound with _sound.
            _volumeCalculator.OnSoundBegan();
        }

        private void FindAudioSource(AudiobreadSource audiobreadSource)
        {
            if (_initSoundSource) return;
            _audiobreadSource = audiobreadSource;
            _audioSource = _audiobreadSource.gameObject.GetComponent<AudioSource>();
            _initSoundSource = true;
        }

        protected override void OnUnityAudioGeneratorTick(ref Sound.SoundInstancePlaybackInfo instancePlaybackInfo)
        {
            ApplySoundModifiers(ref instancePlaybackInfo, SetValuesType.OnUpdate);
        }

        protected override void ApplySoundModifiers(ref Sound.SoundInstancePlaybackInfo instancePlaybackInfo, SetValuesType setValuesType, 
            PlaySoundFlags playSoundFlags = PlaySoundFlags.None)
        {
            if (!MySound.IsValid() || !_initSoundSource) return;
            
            var properties = _soundPropertiesBySetType[setValuesType];

            for (var i = 0; i < properties.Length; ++i)
            {
                if (!_calculators[properties[i]].Active) continue;
                
                // Debug.Log($"{setValuesType}--{properties.Length} {properties[i].ShortName}");
                
                // Pitch
                if (properties[i] == CoreUnitySoundModuleDefinition.PitchProperty)
                {
                    var value = _calculators[properties[i]].ValueContainer.FloatValue;
                    _audioSource.pitch = value;
                    continue;
                }
                
                // Volume
                if (properties[i] == CoreUnitySoundModuleDefinition.VolumeProperty)
                {
                    var value = _calculators[properties[i]].ValueContainer.FloatValue;
                    _audioSource.volume = value;
                    continue;
                }

                // Delay
                if (properties[i] == CoreUnitySoundModuleDefinition.DelayProperty)
                {
                    if (setValuesType == SetValuesType.OnBeforePlay)
                    {
                        var delay = _calculators[properties[i]].ValueContainer.FloatValue;
                        if (delay > 0)
                        {
                            if (instancePlaybackInfo.startTime < AudioSettings.dspTime)
                            {
                                instancePlaybackInfo.startTime = AudioSettings.dspTime;
                            }
                            instancePlaybackInfo.startTime += delay;
                            instancePlaybackInfo.scheduledStart = true;
                        }
                    }
                    continue;
                }
                
                // Time duration
                if (properties[i] == CoreUnitySoundModuleDefinition.TimeDurationProperty)
                {
                    if (setValuesType == SetValuesType.OnBeforePlay)
                    {
                        // TODO: Validate the value?
                        var duration = _calculators[properties[i]].ValueContainer.DoubleValue;
                        // Debug.Log($"Setting duration to {duration}");
                        instancePlaybackInfo.duration = duration;
                        instancePlaybackInfo.scheduledEnd = true;
                        instancePlaybackInfo.scheduledStart = true;
                    }

                    continue;
                }
                
                // TODO: Add offset.
                // else if (properties[i] == _offsetProperty)
                // {
                //     var audioClip = _soundDefinition.AudioClip;
                //     _audioSource.timeSamples = TimeSamplesHelper.ValidateAudioClipOffset(in audioClip, value);
                // }
            }
        }

        #region IStopControllerProcessor Methods
        public bool CanStopNow(StopSoundFlags stopSoundFlags)
        {
            return Sound.HasStopFlag(stopSoundFlags, StopSoundFlags.Instant) || !_hasFadeOut;
        }
        
        public void RequestStop(StopSoundFlags stopSoundFlags, StopControllerStopCallback stopCallback)
        {
            if (CanStopNow(stopSoundFlags))
            {
                stopCallback();
                return;
            }
            
            if (!_volumeCalculator.OnStopSound(stopSoundFlags))
            {
                stopCallback();
                return;
            }

            _stopCallbackWaiting = stopCallback;
        }
        #endregion
    }
}