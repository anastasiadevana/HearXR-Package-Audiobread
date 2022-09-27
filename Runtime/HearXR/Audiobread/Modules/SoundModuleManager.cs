using System;
using System.Collections.Generic;
using UnityEngine;

// TODO: Add a Unity main menu item to rescan all the resources for Sound Modules and properties.
namespace HearXR.Audiobread
{
    [CreateAssetMenu (menuName = "Audiobread/Sound Modules/Sound Module Manager")]
    public class SoundModuleManager : ScriptableObject
    {
        #region Private Fields
        private Dictionary<Type, SoundModule> _soundModulesByType = new Dictionary<Type, SoundModule>();
        #endregion
        
        #region Properties
        public List<SoundModule> SoundModules => _soundModules;
        private List<SoundModule> _soundModules = new List<SoundModule>(); 
        #endregion
        
        #region Init
        public void Awake()
        {
            Init();
        }
        #endregion

        #region Public Methods
        public void InitPoolItemTemplate(ref AudiobreadSource poolItemTemplate)
        {
            foreach (var soundModule in _soundModules)
            {
                soundModule.HandleInitPoolItem(ref poolItemTemplate);
            }
        }

        public SoundModule GetSoundModuleByType(Type type) 
        {
            if (_soundModulesByType == null || _soundModulesByType.Count == 0)
            {
                Init();
            }

            return _soundModulesByType[type];
        }
        
        public T GetModuleByType<T>() where T : SoundModule
        {
            return (T) GetSoundModuleByType(typeof(T));
        }
        
        public List<SoundModule> GetCompatibleModules(ISoundDefinition soundDefinition)
        {
            var result = new List<SoundModule>();
            
            foreach (var soundModule in _soundModules)
            {
                if (soundModule.IsCompatibleWith(soundDefinition))
                {
                    result.Add(soundModule);
                }
            }

            return result;
        }
        #endregion
        
        #region Private Methods
        private void Init()
        {
            var soundModules = Resources.LoadAll(string.Empty, typeof(SoundModule));

            foreach (var soundModuleAsset in soundModules)
            {
                var soundModule = (SoundModule) soundModuleAsset;
                
                var type = soundModule.GetType();
                
                if (!_soundModules.Contains(soundModule))
                {
                    _soundModules.Add(soundModule);
                }
                
                if (_soundModulesByType.ContainsKey(type))
                {
                    Debug.LogError($"Sound module {type} is registered twice. this is really bad.");
                    continue;
                }
                _soundModulesByType.Add(type, soundModule);
            }
        }
        #endregion
    }
}