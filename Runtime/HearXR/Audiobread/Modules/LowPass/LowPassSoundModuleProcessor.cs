using UnityEngine;

namespace HearXR.Audiobread
{
    public class LowPassSoundModuleProcessor : SoundModuleProcessor<LowPassSoundModule, LowPassSoundModuleDefinition>
    {
        #region Private Fields
        private bool _initComplete = false;
        private AudioLowPassFilter _lowPassFilter;
        private bool _invalid = false;
        #endregion
        
        public LowPassSoundModuleProcessor(LowPassSoundModule soundModule, ISound sound) : base(soundModule, sound) {}
        
        protected override void DoApplySoundDefinitionToUnityAudio(AudiobreadSource audiobreadSource)
        {
            base.DoApplySoundDefinitionToUnityAudio(audiobreadSource);
            InitLowPass(audiobreadSource);
        }
        
        protected override void ApplySoundModifiers(ref Sound.SoundInstancePlaybackInfo instancePlaybackInfo, SetValuesType setValuesType, 
            PlaySoundFlags playSoundFlags = PlaySoundFlags.None)
        {
            if (!MySound.IsValid() || !_initComplete || _invalid) return;
            
            var properties = _soundPropertiesBySetType[setValuesType];
            
            for (var i = 0; i < properties.Length; ++i)
            {
                if (properties[i] == LowPassSoundModuleDefinition.LowPassCutoffFrequencyProperty)
                {
                    var value = _calculators[properties[i]].ValueContainer.FloatValue;
                    _lowPassFilter.cutoffFrequency = value;
                }
            
                if (properties[i] == LowPassSoundModuleDefinition.LowPassResonanceQProperty)
                {
                    var value = _calculators[properties[i]].ValueContainer.FloatValue;
                    _lowPassFilter.lowpassResonanceQ = value;
                }
            }
        }
        
        #region Private Methods
        private void InitLowPass(AudiobreadSource audiobreadSource)
        {
            if (_initComplete) return;
            _lowPassFilter = audiobreadSource.gameObject.GetComponent<AudioLowPassFilter>();
            if (_lowPassFilter == null)
            {
                Debug.LogError($"Low pass effect not found on {audiobreadSource}");
                _invalid = true;
            }
            else
            {
                // Debug.Log($"Enable low pass on {MySound}");
                _lowPassFilter.enabled = true;
            }
            _initComplete = true;
        }
        #endregion
    }
}