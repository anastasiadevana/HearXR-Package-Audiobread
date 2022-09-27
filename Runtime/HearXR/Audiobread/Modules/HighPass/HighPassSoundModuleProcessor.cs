using UnityEngine;

namespace HearXR.Audiobread
{
    public class HighPassSoundModuleProcessor : SoundModuleProcessor<HighPassSoundModule, HighPassSoundModuleDefinition>
    {
        private bool _initSoundSource;
        private AudiobreadSource _audiobreadSource;
        private AudioHighPassFilter _highPassFilter;
        
        public HighPassSoundModuleProcessor(HighPassSoundModule soundModule, ISound sound) : base(soundModule, sound) {}

        protected override void DoApplySoundDefinitionToUnityAudio(AudiobreadSource audiobreadSource)
        {
            base.DoApplySoundDefinitionToUnityAudio(audiobreadSource);
            
            FindLowPassFilter(audiobreadSource);

            AdjustHighPassFilter();
        }

        private void FindLowPassFilter(AudiobreadSource audiobreadSource)
        {
            if (_initSoundSource) return;
            _audiobreadSource = audiobreadSource;
            _highPassFilter = _audiobreadSource.gameObject.GetComponent<AudioHighPassFilter>();
            _initSoundSource = true;
        }

        private void AdjustHighPassFilter()
        {
            _highPassFilter.enabled = !ModuleSoundDefinition.bypass;
        }

        protected override void ApplySoundModifiers(SetValuesType setValuesType, PlaySoundFlags playSoundFlags)
        {
            throw new System.NotImplementedException();
        }
    }
}