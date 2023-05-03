using UnityEngine;

namespace HearXR.Audiobread
{
    // [CreateAssetMenu (menuName = "Audiobread/Sound Modules/Low Pass")]
    public class LowPassSoundModule : SoundModule
    {
        #region Sound Module Abstract Properties
        public override string DisplayName => "Low Pass";
        public override bool EnabledByDefault => false;
        public override bool PropagateToChildren => true;
        #endregion
        
        #region Sound Module Abstract Methods
        /// <summary>
        /// Cannot iterate through the list of types, because C# doesn't like it.
        /// </summary>
        /// <param name="soundDefinition"></param>
        /// <returns></returns>
        public override bool IsCompatibleWith(in ISoundDefinition soundDefinition)
        {
            if (soundDefinition is SoundDefinition) return true;
            return false;
        }

        public override bool IsCompatibleWithChild(in ISoundDefinition soundDefinition)
        {
            if (soundDefinition is ISoundGeneratorUnityAudioDefinition) return true;
            return false;
        }

        public override SoundModuleDefinition CreateModuleSoundDefinition()
        {
            var soundModuleProperties = CreateInstance<LowPassSoundModuleDefinition>();
            soundModuleProperties.name = DisplayName;
            soundModuleProperties.soundModule = this;
            return soundModuleProperties;
        }

        public override SoundModuleProcessor CreateSoundModuleProcessor(ISound sound)
        {
            return new LowPassSoundModuleProcessor(this, sound);
        }
        #endregion
        
        #region Sound Module Virtual Methods
        public override void HandleInitPoolItem(ref AudiobreadSource audiobreadSource)
        {
            base.HandleInitPoolItem(ref audiobreadSource);
            
            // See if the item already has a low pass effect.
            var lpFilter = audiobreadSource.GetComponent<AudioLowPassFilter>();
            if (lpFilter != null) return;
            lpFilter = audiobreadSource.gameObject.AddComponent<AudioLowPassFilter>();
            lpFilter.enabled = false;
        }
        #endregion
    }
}