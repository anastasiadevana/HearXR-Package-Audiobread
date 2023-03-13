#if MSA
using MSA;
#endif

using UnityEngine;

namespace HearXR.Audiobread
{
    [CreateAssetMenu (menuName = "Audiobread/Sound Modules/MSA")]
    public class MSASoundModule : SoundModule
    {
        #region Sound Module Abstract Properties
        public override string DisplayName => "MSA";

        public override bool EnabledByDefault => (AudioSettings.GetSpatializerPluginName() == "MSA Spatializer");
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
            var soundModuleProperties = CreateInstance<MSASoundModuleDefinition>();
            soundModuleProperties.name = DisplayName;
            soundModuleProperties.soundModule = this;
            return soundModuleProperties;
        }

        public override SoundModuleProcessor CreateSoundModuleProcessor(ISound sound)
        {
            return new MSASoundModuleProcessor(this, sound);
        }
        #endregion
        
        #region Sound Module Virtual Methods
        public override void HandleInitPoolItem(ref AudiobreadSource audiobreadSource)
        {
            base.HandleInitPoolItem(ref audiobreadSource);
            
#if MSA
            // See if the item already has an MSA Source.
            var msaSource = audiobreadSource.GetComponent<MSASource>();
            if (msaSource != null) return;
            
            msaSource = audiobreadSource.gameObject.AddComponent<MSASource>();
            msaSource.enabled = false;
#endif
        }
        #endregion
    }
}
