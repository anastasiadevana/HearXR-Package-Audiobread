using UnityEngine;

namespace HearXR.Audiobread
{
    [CreateAssetMenu (menuName = "Audiobread/Sound Modules/Tone Generator")]
    public class ToneGeneratorSoundModule : SoundModule
    {
        #region Sound Module Abstract Properties
        public override string DisplayName => "Tone Generator";
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
            if (soundDefinition is IToneGeneratorSoundDefinition) return true;
            return false;
        }

        public override bool IsCompatibleWithChild(in ISoundDefinition soundDefinition)
        {
            return false;
        }

        public override SoundModuleDefinition CreateModuleSoundDefinition()
        {
            var soundModuleProperties = CreateInstance<ToneGeneratorSoundModuleDefinition>();
            soundModuleProperties.name = DisplayName;
            soundModuleProperties.soundModule = this;
            return soundModuleProperties;
        }

        public override SoundModuleProcessor CreateSoundModuleProcessor(ISound sound)
        {
            return new ToneGeneratorSoundModuleProcessor(this, sound);
        }
        #endregion
    }
}