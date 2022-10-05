using UnityEngine;

namespace HearXR.Audiobread.Core
{
    // TODO: Is this even used? Or are we using CoreUnitySoundModule instead?
    [CreateAssetMenu (menuName = "Audiobread/Sound Modules/Core Unity Audio")]
    public class CoreSoundModule : SoundModule
    {
        #region Sound Module Abstract Properties
        public override string DisplayName => "Core";
        public override bool EnabledByDefault => true;
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
            var soundModuleProperties = CreateInstance<CoreSoundModuleDefinition>();
            soundModuleProperties.name = DisplayName;
            soundModuleProperties.soundModule = this;
            return soundModuleProperties;
        }

        public override SoundModuleProcessor CreateSoundModuleProcessor(ISound sound)
        {
            return new CoreSoundProcessor(this, sound);
        }
        #endregion
    }
}