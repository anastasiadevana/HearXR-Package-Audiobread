using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Audio;

[assembly: InternalsVisibleTo("HearXR.Audiobread.Editor")]
namespace HearXR.Audiobread
{
    /// <summary>
    /// Base sound definition for any sound.
    /// </summary>
    public abstract class SoundDefinition : ScriptableObject, ISoundDefinition
    {
        #region Editor Fields
        [TextArea(1, 10)] public string soundDesignerNotes;

        public float chanceToPlay = 1.0f;
        
        // TODO: Move to Unity one.
        public AudioMixerGroup audioMixerGroup;
        
        [SerializeField] private bool _pitched = false;

        [SerializeField] private int _baseNoteNumber;

        [SerializeField] protected List<SoundModuleDefinition> _moduleSoundDefinitions = new List<SoundModuleDefinition>();
        
        [SerializeField, HideInInspector] private List<SoundModule> _enabledSoundModules = new List<SoundModule>();
        #endregion
        
        #region ISoundDefinition Properties
        public bool WasChanged
        {
            get => wasChanged;
            set => wasChanged = value;
        }

        public List<SoundModuleDefinition> ModuleSoundDefinitions => _moduleSoundDefinitions;

        public Dictionary<SoundModule, SoundModuleDefinition> PropagatedSoundModuleDefinitions { get; set; } = new Dictionary<SoundModule, SoundModuleDefinition>();

        public string Name => name;

        // TODO: Use this.
        public float ChanceToPlay
        {
            get => chanceToPlay;
            set => chanceToPlay = value;
        }

        public string SoundDesignerNotes
        {
            get => soundDesignerNotes;
            set => soundDesignerNotes = value;
        }

        public List<SoundModule> EnabledModules => _enabledSoundModules;

        public List<SoundModule> GetCompatibleModules()
        {
            return BuiltInData.SoundModuleManager.GetCompatibleModules(this);
        }
        
        public bool Pitched => _pitched;
        public int BaseNoteNumber => _baseNoteNumber;
        #endregion

        #region ISoundDefinition Methods
        public virtual ISound CreateSound(InitSoundFlags initSoundFlags = InitSoundFlags.None)
        {
            return CreateSound<ISound>(initSoundFlags);
        }
        
        public abstract T CreateSound<T>(InitSoundFlags initSoundFlags = InitSoundFlags.None) where T : class, ISound;
        #endregion

        #region Helper Fields
        [System.NonSerialized] public bool loop;
        
        [System.NonSerialized] public int loopCount;
        
        [HideInInspector] public bool wasChanged;
        #endregion
        
        internal void RescanEnabledModules()
        {
            _enabledSoundModules.Clear();
            foreach (var m in _moduleSoundDefinitions)
            {
                _enabledSoundModules.Add(m.soundModule);
            }
        }
        
        internal bool ModuleEnabled(SoundModule module)
        {
            return _enabledSoundModules.Contains(module);
        }
        
        internal SoundModuleDefinition AddModule(SoundModule module)
        {
            if (_enabledSoundModules.Contains(module)) return null;
            _enabledSoundModules.Add(module);
            var soundModuleSoundDefinition = module.CreateModuleSoundDefinition();
            _moduleSoundDefinitions.Add(soundModuleSoundDefinition);
            return soundModuleSoundDefinition;
        }

        internal SoundModuleDefinition RemoveModule(SoundModule module)
        {
            SoundModuleDefinition smp = null;
            _enabledSoundModules.Remove(module);
            for (var i = _moduleSoundDefinitions.Count - 1; i >= 0; --i)
            {
                if (_moduleSoundDefinitions[i].soundModule == module)
                {
                    smp = _moduleSoundDefinitions[i];
                    _moduleSoundDefinitions.RemoveAt(i);
                    break;
                }
            }

            return smp;
        }
        
        #region Validate
        internal virtual void OnDefaultModulesAdded() {}
        #endregion
    }
}
