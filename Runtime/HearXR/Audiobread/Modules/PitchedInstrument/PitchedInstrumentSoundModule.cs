using UnityEngine;

namespace HearXR.Audiobread
{
    [CreateAssetMenu(menuName = "Audiobread/Sound Modules/Pitched Instrument")]
    public class PitchedInstrumentSoundModule : SoundModule
    {
        #region Sound Module Abstract Properties
        public override string DisplayName => "Musical Pitch";
        public override bool EnabledByDefault => true;
        #endregion
        
        #region Sound Module Abstract Methods
        public override bool IsCompatibleWith(in ISoundDefinition soundDefinition)
        {
            if (soundDefinition is IPitchedInstrumentSoundDefinition) return true;
            return false;
        }

        public override SoundModuleDefinition CreateModuleSoundDefinition()
        {
            var soundModuleProperties = CreateInstance<PitchedInstrumentSoundModuleDefinition>();
            soundModuleProperties.name = DisplayName;
            soundModuleProperties.soundModule = this;
            return soundModuleProperties;
        }

        public override SoundModuleProcessor CreateSoundModuleProcessor(ISound sound)
        {
            return new PitchedInstrumentSoundModuleProcessor(this, sound);
        }
        #endregion
    }
}