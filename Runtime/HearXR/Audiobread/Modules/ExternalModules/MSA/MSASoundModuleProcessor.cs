#if MSA
using MSA;
#endif

namespace HearXR.Audiobread
{
    public class MSASoundModuleProcessor : SoundModuleProcessor<MSASoundModule, MSASoundModuleDefinition>
    {
        private bool _initSoundSource;
        private AudiobreadSource _audiobreadSource;
#if MSA
        private MSASource _msaSource;
#endif
        
        public MSASoundModuleProcessor(MSASoundModule soundModule, ISound sound) : base(soundModule, sound) {}

        protected override void DoApplySoundDefinitionToUnityAudio(AudiobreadSource audiobreadSource)
        {
            base.DoApplySoundDefinitionToUnityAudio(audiobreadSource);
#if MSA
            FindMSASource(audiobreadSource);
            ApplySpatializationSettings();
#endif
        }

        private void FindMSASource(AudiobreadSource audiobreadSource)
        {
            if (_initSoundSource) return;
            _audiobreadSource = audiobreadSource;
#if MSA
            _msaSource = _audiobreadSource.gameObject.GetComponent<MSASource>();
#endif
            _initSoundSource = true;
        }

        private void ApplySpatializationSettings()
        {
#if MSA
            if (!ModuleSoundDefinition.bypass)
            {
                _audiobreadSource.AudioSource.spatialize = true;
                _audiobreadSource.AudioSource.spatialBlend = 1.0f;
                
                _msaSource.enabled = true;
                
                // TODO: Apply MSA module properties.
            }
            else
            {
                _msaSource.enabled = false;
            }
#endif
        }

        protected override void ApplySoundModifiers(ref Sound.SoundInstancePlaybackInfo instancePlaybackInfo, SetValuesType setValuesType, 
            PlaySoundFlags playSoundFlags = PlaySoundFlags.None)
        {
#if MSA
            // TODO:
#endif
        }
    }
}
