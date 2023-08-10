using System.Collections.Generic;
using UnityEngine;

namespace HearXR.Audiobread
{
    // [CreateAssetMenu (menuName = "Audiobread/Sound Modules/Core Child Picker")]
    public class CoreChildPickerSoundModule : SoundModule, IChildPickerModule
    {
        #region Sound Module Abstract Properties
        public override string DisplayName => "Core Child Picker";
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
            if (soundDefinition is ContainerSoundDefinition) return true;
            return false;
        }

        public override bool IsCompatibleWithChild(in ISoundDefinition soundDefinition)
        {
            return false;
        }

        public override SoundModuleProcessor CreateSoundModuleProcessor(ISound sound)
        {
            return new CoreChildPickerSoundProcessor(this, sound);
        }
        #endregion
    }
}