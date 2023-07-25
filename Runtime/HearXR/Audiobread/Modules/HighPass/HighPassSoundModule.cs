using UnityEngine;

namespace HearXR.Audiobread
{
    // [CreateAssetMenu (menuName = "Audiobread/Sound Modules/High Pass")]
    public class HighPassSoundModule : SoundModule
    {
        #region Sound Module Abstract Properties
        public override string DisplayName => "High Pass";
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

        public override SoundModuleProcessor CreateSoundModuleProcessor(ISound sound)
        {
            return new HighPassSoundModuleProcessor(this, sound);
        }
        #endregion
        
        #region Sound Module Virtual Methods
        public override void HandleInitPoolItem(ref AudiobreadSource audiobreadSource)
        {
            base.HandleInitPoolItem(ref audiobreadSource);
            
            // See if the item already has a high pass effect.
            var hpFilter = audiobreadSource.GetComponent<AudioHighPassFilter>();
            if (hpFilter != null) return;
            hpFilter = audiobreadSource.gameObject.AddComponent<AudioHighPassFilter>();
            hpFilter.enabled = false;
        }
        #endregion
    }
}