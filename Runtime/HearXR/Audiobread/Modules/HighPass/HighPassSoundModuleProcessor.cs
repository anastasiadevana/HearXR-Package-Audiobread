using UnityEngine;

namespace HearXR.Audiobread
{
    public class HighPassSoundModuleProcessor : SoundModuleProcessor<HighPassSoundModule, HighPassSoundModuleDefinition>
    {
        private bool _initComplete;
        private AudioHighPassFilter _highPassFilter;
        private bool _invalid;
        
        public HighPassSoundModuleProcessor(HighPassSoundModule soundModule, ISound sound) : base(soundModule, sound) {}

        protected override void DoApplySoundDefinitionToUnityAudio(AudiobreadSource audiobreadSource)
        {
            base.DoApplySoundDefinitionToUnityAudio(audiobreadSource);
            InitHighPassFilter(audiobreadSource);
        }

        private void InitHighPassFilter(AudiobreadSource audiobreadSource)
        {
            if (_initComplete) return;
            _highPassFilter = audiobreadSource.gameObject.GetComponent<AudioHighPassFilter>();
            if (_highPassFilter == null)
            {
                Debug.LogError($"High pass effect not found on {audiobreadSource}");
                _invalid = true;
            }
            else
            {
                _highPassFilter.enabled = !ModuleSoundDefinition.bypass;
            }
            _initComplete = true;
        }

        protected override void ApplySoundModifiers(ref Sound.SoundInstancePlaybackInfo instancePlaybackInfo, SetValuesType setValuesType, 
            PlaySoundFlags playSoundFlags = PlaySoundFlags.None)
        {
            if (!MySound.IsValid() || !_initComplete || _invalid) return;
            
            // TODO: Copy the rest from low pass module
            
            throw new System.NotImplementedException();
        }
    }
}