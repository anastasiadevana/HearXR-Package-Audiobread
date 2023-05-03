using System.Linq;
using UnityEngine;

namespace HearXR.Audiobread
{
    [CreateAssetMenu(fileName = "Container", menuName = "Audiobread/Container Sound", order = 0)]
    public class ContainerSoundDefinition : BaseContainerSoundDefinition
    {
        #region Editor Fields
        [SerializeField] protected SoundDefinition[] _children;
        //[SerializeField] 
        // [SerializeField] protected ParentSoundPlaybackOrder _playbackOrder;
        // [SerializeField] protected bool _doNotRepeatLast;
        #endregion

        #region Non-Serialised Fields
        // [System.NonSerialized] private int _definitionSharedLastIndex;
        #endregion
        
        #region ISoundContainerDefinition Properties
        public override int ChildCount => _children.Length;

        // public override int DefinitionSharedLastIndex
        // {
        //     get => _definitionSharedLastIndex;
        //     set => _definitionSharedLastIndex = value;
        // }

        // public override ParentSoundPlaybackOrder PlaybackOrder => _playbackOrder;
        //
        // public override bool DoNotRepeatLast { get; }
        #endregion
        
        #region Hidden Serialized Fields
        // TODO: Hide in inspector later, when we're sure everything is working.
        [SerializeField/*, HideInInspector*/] protected CoreSchedulerSoundModuleDefinition _childSchedulerDefinition;
        #endregion
        
        #region Properties
        public CoreSchedulerSoundModuleDefinition ChildSchedulerDefinition => _childSchedulerDefinition;
        #endregion
        
        #region ISoundContainerDefinition Methods
        public override ISoundDefinition[] GetChildren()
        {
            return _children.ToArray<ISoundDefinition>();
        }
        
        public override int GetNextChildIndex(int lastChildIndex = -1)
        {
            // TODO: A separate module should take care of that.
            throw new System.NotImplementedException();
        }
        #endregion

        #region SoundDefinition Abstract Methods
        public override T CreateSound<T>(InitSoundFlags initSoundFlags = InitSoundFlags.None)
        {
            var sound = new ContainerSound();
            ((ISoundInternal<ContainerSoundDefinition>) sound).Init(this, initSoundFlags);
            return sound as T;
        }
        #endregion

        #region Validate
        internal override void OnDefaultModulesAdded()
        {
            base.OnDefaultModulesAdded();
            ConnectChildSchedulerDefinition();
        }

        private void ConnectChildSchedulerDefinition()
        {
            if (_childSchedulerDefinition != null) return;
            
            for (var i = 0; i < ModuleSoundDefinitions.Count; ++i)
            {
                if (!(ModuleSoundDefinitions[i] is CoreSchedulerSoundModuleDefinition)) continue;
                _childSchedulerDefinition = ModuleSoundDefinitions[i] as CoreSchedulerSoundModuleDefinition;
                return;
            }
        }
        #endregion
    }
}