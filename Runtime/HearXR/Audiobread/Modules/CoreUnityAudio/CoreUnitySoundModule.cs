using UnityEngine;

namespace HearXR.Audiobread.Core
{
    [CreateAssetMenu (menuName = "Audiobread/Sound Modules/Core Unity Audio")]
    public class CoreUnitySoundModule : SoundModule
    {
        #region Sound Module Abstract Properties
        public override string DisplayName => "Core Unity";
        public override bool EnabledByDefault => true;
        public override bool PropagateToChildren => false;
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
            return false;
        }

        public override SoundModuleDefinition CreateModuleSoundDefinition()
        {
            var soundModuleProperties = CreateInstance<CoreUnitySoundModuleDefinition>();
            soundModuleProperties.name = DisplayName;
            soundModuleProperties.soundModule = this;
            return soundModuleProperties;
        }

        public override SoundModuleProcessor CreateSoundModuleProcessor(ISound sound)
        {
            return new CoreUnitySoundProcessor(this, sound);
        }
        #endregion
    }
}