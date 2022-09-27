using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Audio;

[assembly: InternalsVisibleTo("AudiobreadEditor")]
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

        [Space(20)] public List<SoundParameter> _parameters;
        
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

        public string Name => name;
        
        // public VolumeDefinition VolumeDefinition => _volumeDefinition;
        
        public List<SoundParameter> Parameters => _parameters;

        public List<SoundModule> EnabledModules => _enabledSoundModules;

        public List<SoundModule> GetCompatibleModules()
        {
            return BuiltInData.SoundModuleManager.GetCompatibleModules(this);
            // CacheCompatibleModules();
            // return _compatibleSoundModules;
        }
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
            // Debug.Log($"Just added {module} currently enabled {_enabledSoundModules.Count}");
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