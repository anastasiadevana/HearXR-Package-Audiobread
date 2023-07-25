using HearXR.Audiobread.SoundProperties;
using UnityEngine;

namespace HearXR.Audiobread
{
    // TODO: Do not allow to show, if type is OneShot.
    // [CreateAssetMenu (menuName = "Audiobread/Sound Modules/Core Scheduler")]
    public class CoreSchedulerSoundModule : SoundModule, IChildSchedulerModule
    {
        #region Sound Module Abstract Properties
        public override string DisplayName => "Core Scheduler";
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
            if (soundDefinition is IContainerSoundDefinition) return true;
            return false;
        }

        public override bool IsCompatibleWithChild(in ISoundDefinition soundDefinition)
        {
            return false;
        }

        public override SoundModuleProcessor CreateSoundModuleProcessor(ISound sound)
        {
            return new CoreSchedulerSoundProcessor(this, sound);
        }
        #endregion
    }
}