using UnityEngine;

namespace HearXR.Audiobread
{
    public class CoreChildPickerSoundProcessor : SoundModuleProcessor<CoreChildPickerSoundModule, CoreChildPickerSoundModuleDefinition>, 
        IChildPickerProcessor
    {
        #region Private Fields
        private IContainerSoundDefinition _soundDefinition;
        
        // TODO: Move this higher, rename it, and actually check it.
        private bool _enabled = false;
        #endregion

        #region Constructor
        
        #endregion
        public CoreChildPickerSoundProcessor(CoreChildPickerSoundModule soundModule, ISound sound) : base(soundModule, sound) {}

        #region SoundModuleProcessor Abstract Methods
        protected override void ApplySoundModifiers(ref Sound.SoundInstancePlaybackInfo instancePlaybackInfo, SetValuesType setValuesType, 
            PlaySoundFlags playSoundFlags = PlaySoundFlags.None) {}
        #endregion
        
        #region SoundModuleProcessor Overrides
        protected override void DoInit()
        {
            base.DoInit();
            if (MySound.SoundDefinition is IContainerSoundDefinition)
            {
                _enabled = true;
                _soundDefinition = MySound.SoundDefinition as IContainerSoundDefinition;
            }
        }
        #endregion
        
        public int GetNextChildIndex(int lastChildIndex = -1)
        {
            if (_soundDefinition.ChildCount == 0)
            {
                Debug.LogError("HEAR XR: This sound has no children.");
                return default;
            }
            
            if (_soundDefinition.ChildCount == 1)
            {
                return 0;
            }

            // TODO: ORLY?
            if (_soundDefinition.SoundType == SoundType.OneShot && lastChildIndex == -1)
            {
                lastChildIndex = ModuleSoundDefinition.definitionSharedLastIndex;
            }
            
            int nextIndex = (ModuleSoundDefinition.PlaybackOrder == ParentSoundPlaybackOrder.Random) 
                ? GetRandomChild(lastChildIndex) 
                : GetNextChild(lastChildIndex);
            
            ModuleSoundDefinition.definitionSharedLastIndex = nextIndex;
            
            return nextIndex;
        }
        
        private int GetRandomChild(int lastIndex)
        {
            if (!ModuleSoundDefinition.DoNotRepeatLast)
            {
                return Random.Range(0, _soundDefinition.ChildCount);
            }
            
            // TODO: Make sure this is an efficient way to do this.
            int i;
            do
            {
                i = Random.Range(0, _soundDefinition.ChildCount);
            } while (i == lastIndex);
            
            return i;
        }
        
        private int GetNextChild(int lastIndex)
        {
            int i = lastIndex + 1;
            if (i >= _soundDefinition.ChildCount)
            {
                i = 0;
            }
            return i;
        }
    }
}