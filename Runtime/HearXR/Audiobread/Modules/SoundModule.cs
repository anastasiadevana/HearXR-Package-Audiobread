using UnityEngine;
using System.Collections.Generic;
using HearXR.Audiobread.SoundProperties;

namespace HearXR.Audiobread
{
    public abstract class SoundModule : ScriptableObject, ISoundModule, ISoundModule<SoundModuleDefinition, SoundModuleProcessor>
    {
        // #region Editor Fields
        // [SerializeField] private List<SoundProperty> _soundProperties = new List<SoundProperty>();
        // #endregion
        
        // #region Properties
        // public List<SoundProperty> SoundProperties => _soundProperties;
        // #endregion
        
        #region Abstract Properties
        public abstract string DisplayName { get; }
        public abstract bool EnabledByDefault { get; }
        #endregion
        
        #region Abstract Methods
        public abstract bool IsCompatibleWith(in ISoundDefinition soundDefinition);
        public abstract SoundModuleDefinition CreateModuleSoundDefinition();
        public abstract SoundModuleProcessor CreateSoundModuleProcessor(ISound sound);
        #endregion
        
        #region Virtual Methods
        public virtual void HandleInitPoolItem(ref AudiobreadSource poolItem) {}
        #endregion
    }
}