using UnityEngine;

namespace HearXR.Audiobread
{
    public abstract class BaseContainerSoundDefinition : SoundDefinition, IContainerSoundDefinition
    {
        #region Editor Fields
        [SerializeField] private SoundType _soundType;
        #endregion
        
        #region Properties
        public SoundType SoundType => _soundType;
        #endregion
        
        public abstract ISoundDefinition[] GetChildren();

        public abstract int ChildCount { get; }
        
        public abstract int GetNextChildIndex(int lastChildIndex = -1);
        
        // public abstract int DefinitionSharedLastIndex { get; set; }
        
        // public abstract ParentSoundPlaybackOrder PlaybackOrder { get; }
        //
        // public abstract bool DoNotRepeatLast { get; }
    }
}