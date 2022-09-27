using System;
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
            _volumeCalculator = (VolumeCalculator) _calculators[ModuleSoundDefinition.VolumeProperty];
            _volumeCalculator.SetFades(ModuleSoundDefinition.fadeInDefinition, ModuleSoundDefinition.fadeOutDefinition);
            _volumeCalculator.OnFadeOutFinished += HandleFadeOutFinished;
            _volumeCalculator.OnFadeOutFinished += HandleFadeInFinished;
            
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
            // Debug.Log($"{sound} began!");
            _volumeCalculator.OnSoundBegan();
        }

        private void FindAudioSource(AudiobreadSource audiobreadSource)
        {
            if (_initSoundSource) return;
            _audiobreadSource = audiobreadSource;
            _audioSource = _audiobreadSource.gameObject.GetComponent<AudioSource>();
            _initSoundSource = true;
        }

        protected override void OnUnityAudioGeneratorTick()
        {
            ApplySoundModifiers(SetValuesType.OnUpdate);
        }

        protected override void ApplySoundModifiers(SetValuesType setValuesType, PlaySoundFlags playSoundFlags = PlaySoundFlags.None)
        {
            if (!MySound.IsValid() || !_initSoundSource) return;
            
            var properties = _soundPropertiesBySetType[setValuesType];

            for (int i = 0; i < properties.Length; ++i)
            {
                if (properties[i] == ModuleSoundDefinition.PitchProperty)
                {
                    var value = _calculators[properties[i]].ValueContainer.FloatValue;
                    _audioSource.pitch = value;
                }

                if (properties[i] == ModuleSoundDefinition.VolumeProperty)
                {
                    var value = _calculators[properties[i]].ValueContainer.FloatValue;
                    _audioSource.volume = value;
                }
                
                // if (!_calculators.ContainsKey(properties[i]))
                // {
                //     Debug.LogError($"HEAR XR {this} doesn't have the calculator for {properties[i].name}");
                // }
                //
                // _calculators[properties[i]].Calculate();
                // var value = _calculators[properties[i]].ValueContainer.FloatValue;
                //
                // *if (properties[i] == _volumeProperty)
                // *{
                // *    _audioSource.volume = value;
                // *}
                // *else if (properties[i] == _pitchProperty)
                // *{
                // *    _audioSource.pitch = value;
                // *}
                // else if (properties[i] == _delayProperty)
                // {
                //     continue; // Delay is used at the moment of playing.
                // }
                // else if (properties[i] == _offsetProperty)
                // {
                //     var audioClip = _soundDefinition.AudioClip;
                //     _audioSource.timeSamples = TimeSamplesHelper.ValidateAudioClipOffset(in audioClip, value);
                // }
                // else
                // {
                //     Debug.LogWarning($"HEAR XR: {this} Unable to apply property {properties[i].name}");
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