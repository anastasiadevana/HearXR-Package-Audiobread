using System;
using UnityEngine;

namespace HearXR.Audiobread
{
    public abstract class SoundModule : ScriptableObject, ISoundModule<SoundModuleDefinition, SoundModuleProcessor>
    {
        #region Editor Fields
        [SerializeField] private SoundModuleDefinition _defaultSoundModuleDefinition;
        #endregion
        
        #region Properties
        public SoundModuleDefinition DefaultSoundModuleDefinition
        {
            get
            {
                if (_defaultSoundModuleDefinition == null)
                {
                    throw new Exception($"{name} module doesn't have a default!");
                }
                _defaultSoundModuleDefinition.soundModule = this;
                return _defaultSoundModuleDefinition;
            }
        }
        #endregion
        
        #region Abstract Properties
        public abstract string DisplayName { get; }
        public abstract bool EnabledByDefault { get; }
        public abstract bool PropagateToChildren { get; }
        #endregion
        
        #region Abstract Methods
        public abstract bool IsCompatibleWith(in ISoundDefinition soundDefinition);
        public abstract bool IsCompatibleWithChild(in ISoundDefinition soundDefinition);
        // public abstract SoundModuleDefinition CreateModuleSoundDefinition();
        public abstract SoundModuleProcessor CreateSoundModuleProcessor(ISound sound);
        #endregion
        
        #region Unity Methods
        private void OnValidate()
        {
            if (_defaultSoundModuleDefinition != null)
            {
                _defaultSoundModuleDefinition.soundModule = this;
            }
        }
        #endregion
        
        #region Virtual Methods
        public virtual void HandleInitPoolItem(ref AudiobreadSource audiobreadSource) {}
        #endregion
    }
}