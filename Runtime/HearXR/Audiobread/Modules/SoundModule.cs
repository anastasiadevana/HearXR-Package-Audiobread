using UnityEngine;

namespace HearXR.Audiobread
{
    public abstract class SoundModule : ScriptableObject, ISoundModule<SoundModuleDefinition, SoundModuleProcessor>
    {
        #region Abstract Properties
        public abstract string DisplayName { get; }
        public abstract bool EnabledByDefault { get; }
        public abstract bool PropagateToChildren { get; }
        #endregion
        
        #region Abstract Methods
        public abstract bool IsCompatibleWith(in ISoundDefinition soundDefinition);
        public abstract bool IsCompatibleWithChild(in ISoundDefinition soundDefinition);
        public abstract SoundModuleDefinition CreateModuleSoundDefinition();
        public abstract SoundModuleProcessor CreateSoundModuleProcessor(ISound sound);
        #endregion
        
        #region Virtual Methods
        public virtual void HandleInitPoolItem(ref AudiobreadSource audiobreadSource) {}
        #endregion
    }
}