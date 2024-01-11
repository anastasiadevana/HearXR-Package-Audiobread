using UnityEngine;

namespace HearXR.Audiobread
{
    public class EchoSoundModuleProcessor : SoundModuleProcessor<EchoSoundModule, EchoSoundModuleDefinition>
    {
        #region Private Fields
        private bool _initComplete = false;
        private AudioEchoFilter _echoFilter;
        private bool _invalid = false;
        #endregion
        
        public EchoSoundModuleProcessor(EchoSoundModule soundModule, ISound sound) : base(soundModule, sound) {}
        
        protected override void DoApplySoundDefinitionToUnityAudio(AudiobreadSource audiobreadSource)
        {
            base.DoApplySoundDefinitionToUnityAudio(audiobreadSource);
            InitEchoFilter(audiobreadSource);
        }
        
        protected override void ApplySoundModifiers(ref Sound.SoundInstancePlaybackInfo instancePlaybackInfo, SetValuesType setValuesType, 
            PlaySoundFlags playSoundFlags = PlaySoundFlags.None)
        {
            if (!MySound.IsValid() || !_initComplete || _invalid) return;
            
            var properties = _soundPropertiesBySetType[setValuesType];
            
            for (var i = 0; i < properties.Length; ++i)
            {
                if (properties[i] == EchoSoundModuleDefinition.EchoDelayProperty)
                {
                    var value = _calculators[properties[i]].ValueContainer.FloatValue;
                    _echoFilter.delay = value;
                }
            
                if (properties[i] == EchoSoundModuleDefinition.EchoDecayRatioProperty)
                {
                    var value = _calculators[properties[i]].ValueContainer.FloatValue;
                    _echoFilter.decayRatio = value;
                }
                
                if (properties[i] == EchoSoundModuleDefinition.EchoDryMixProperty)
                {
                    var value = _calculators[properties[i]].ValueContainer.FloatValue;
                    _echoFilter.dryMix = value;
                }
                
                if (properties[i] == EchoSoundModuleDefinition.EchoWetMixProperty)
                {
                    var value = _calculators[properties[i]].ValueContainer.FloatValue;
                    _echoFilter.wetMix = value;
                }
            }
        }
        
        #region Private Methods
        private void InitEchoFilter(AudiobreadSource audiobreadSource)
        {
            if (_initComplete) return;
            _echoFilter = audiobreadSource.gameObject.GetComponent<AudioEchoFilter>();
            if (_echoFilter == null)
            {
                Debug.LogError($"Echo effect not found on {audiobreadSource}");
                _invalid = true;
            }
            else
            {
                _echoFilter.enabled = !ModuleSoundDefinition.bypass;
            }
            _initComplete = true;
        }
        #endregion
    }
}