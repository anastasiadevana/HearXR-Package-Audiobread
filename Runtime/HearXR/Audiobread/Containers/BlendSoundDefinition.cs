using System.Linq;
using UnityEngine;

namespace HearXR.Audiobread
{
    [CreateAssetMenu(fileName = "Blend", menuName = "Audiobread/Blend Sound", order = 1)]
    public class BlendSoundDefinition : BaseContainerSoundDefinition
    {
        #region Editor Fields
        [SerializeField] protected SoundDefinition[] _children;
        #endregion

        #region ISoundContainerDefinition Properties
        public override int ChildCount => _children.Length;
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
            var sound = new BlendSound();
            ((ISoundInternal<BlendSoundDefinition>) sound).Init(this, initSoundFlags);
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