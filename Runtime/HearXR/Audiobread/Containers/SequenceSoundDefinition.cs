using System.Linq;
using HearXR.Audiobread.SoundProperties;
using UnityEngine;

namespace HearXR.Audiobread
{
    [CreateAssetMenu(fileName = "Sequence", menuName = "Audiobread/Sounds/Sequence Sound")]
    public class SequenceSoundDefinition : BaseContainerSoundDefinition
    {
        public override ISoundDefinition[] GetChildren()
        {
            return _children.ToArray<ISoundDefinition>();
        }

        public override int ChildCount => _children.Length;

        public SoundDefinition[] Children => _children;
        [SerializeField] protected SoundDefinition[] _children;
        
        public override T CreateSound<T>(InitSoundFlags initSoundFlags = InitSoundFlags.None)
        {
            var sound = new SequenceSound();
            ((ISoundInternal<SequenceSoundDefinition>) sound).Init(this, initSoundFlags);
            return sound as T;
        }

        #region Editor Fields

        public RepeatsDefinition repeatsDefinition;

        public TimeBetweenDefinition timeBetweenDefinition;
        
        public ParentSoundType parentSoundType;
        
        // TODO: Show only if more than one child.
        public ParentSoundPlaybackOrder playbackOrder;
        
        // TODO: Show only if more than one clip and random.
        public bool dontRepeatLast = true;
        
        // TODO: RepeatType "Parent" AND PlaybackOrder "Random" is not allowed.
        public ParentSoundRepeatType repeatType;
        #endregion
        
        public override int GetNextChildIndex(int lastChildIndex = -1)
        {
            if (_children.Length == 0)
            {
                Debug.LogError("HEAR XR: This sound has no children.");
                return default;
            }
            
            if (_children.Length == 1)
            {
                return 0;
            }

            // TODO: ORLY?
            // if (_parentSoundType == ParentSoundType.OneShot && lastChildIndex == -1)
            // {
            //     lastChildIndex = _globalLastIndex;
            // }
            
            int index = (playbackOrder == ParentSoundPlaybackOrder.Random) ? GetRandomChild(lastChildIndex) : GetNextChild(lastChildIndex);
            
            _globalLastIndex = index;
            
            return index;
        }

        // public override int DefinitionSharedLastIndex { get; set; }
        // public override ParentSoundPlaybackOrder PlaybackOrder { get; }

        private int GetRandomChild(int lastIndex)
        {
            if (!dontRepeatLast)
            {
                return Random.Range(0, _children.Length);
            }
            
            // TODO: Make sure this is an efficient way to do this.
            int i;
            do
            {
                i = Random.Range(0, _children.Length);
            } while (i == lastIndex);
            
            return i;
        }
        
        private int GetNextChild(int lastIndex)
        {
            int i = lastIndex + 1;
            if (i >= _children.Length)
            {
                i = 0;
            }
            return i;
        }
        
        [System.NonSerialized] private int _globalLastIndex;
    }
}