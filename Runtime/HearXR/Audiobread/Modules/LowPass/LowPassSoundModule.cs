using UnityEngine;

namespace HearXR.Audiobread
{
    [CreateAssetMenu (menuName = "Audiobread/Sound Modules/Low Pass")]
    public class LowPassSoundModule : SoundModule
    {
        #region Sound Module Abstract Properties
        public override string DisplayName => "Low Pass";
        public override bool EnabledByDefault => false;
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
        public override void HandleInitPoolItem(ref AudiobreadSource poolItem)
        {
            base.HandleInitPoolItem(ref poolItem);
            
            // See if the item already has a low pass effect.
            var lowPassEffect = poolItem.GetComponent<AudioLowPassFilter>();
            if (lowPassEffect != null) return;
            var lpFilter = poolItem.gameObject.AddComponent<AudioLowPassFilter>();
            lpFilter.enabled = false;
        }
        #endregion
    }
}