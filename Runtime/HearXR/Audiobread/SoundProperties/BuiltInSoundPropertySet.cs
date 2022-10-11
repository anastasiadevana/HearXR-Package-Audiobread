using System;
using System.Collections.Generic;
using UnityEngine;

// TODO: This file should be renamed, since it's more than a set now.
// TODO: Maybe use constant values of some sort instead of the enum. This will enable plugins.
// TODO: Do we even need this file as a SCRIPTABLE OBJECT? Nothing is assigned in the editor.
namespace HearXR.Audiobread.SoundProperties
{
    [CreateAssetMenu (menuName = "Audiobread/Built In Sound Property Set")]
    public class BuiltInSoundPropertySet : ScriptableObject
    {
        public void Awake()
        {
            InitDataStructures();
        }

        public Dictionary<SetValuesType, SoundProperty[]> PropertiesBySetType
        {
            get
            {
                InitDataStructures();
                return _propertiesBySetType;
            }
        }
        private Dictionary<SetValuesType, SoundProperty[]> _propertiesBySetType = new Dictionary<SetValuesType, SoundProperty[]>();
        
        private Dictionary<Type, SoundProperty> _soundPropertiesBySystemType = new Dictionary<Type, SoundProperty>();
        
        public SoundProperty GetSoundPropertyByType(Type type) 
        {
            if (_soundPropertiesBySystemType == null || _soundPropertiesBySystemType.Count == 0)
            {
                InitDataStructures();
            }
            return _soundPropertiesBySystemType[type];
        }
        
        public T GetSoundPropertyByType<T>() where T : SoundProperty
        {
            return (T) GetSoundPropertyByType(typeof(T));
        }

        public bool GetSoundProperty<T>(out T soundProperty) where T : SoundProperty
        {
            soundProperty = GetSoundPropertyByType<T>();
            if (soundProperty == null)
            {
                Debug.Log($"Property {typeof(T)} not found.");
            }
            return (soundProperty != null);
        }
        
        private void InitDataStructures()
        {
            if (_initComplete) return;
            
            lock (_propertiesBySetType)
            {
                var updatableOnPreparedToPlay = new List<SoundProperty>();
                List<SoundProperty> updatableOnPlay = new List<SoundProperty>();
                List<SoundProperty> updatableOnChildPlay = new List<SoundProperty>();
                List<SoundProperty> continuous = new List<SoundProperty>();
            
                var allProperties = Resources.LoadAll(string.Empty, typeof(SoundProperty));
                for (int i = 0; i < allProperties.Length; ++i)
                {
                    Type type = allProperties[i].GetType();
                    SoundProperty soundProperty = (SoundProperty) allProperties[i];
                 
                    if (_soundPropertiesBySystemType.ContainsKey(type))
                    {
                        Debug.LogError($"Property {type} is registered twice. this is really bad.");
                        continue;
                    }
                
                    _soundPropertiesBySystemType.Add(type, soundProperty);
                    //Debug.Log($"Added {type} to the registry");

                    if (soundProperty.SetValuesOnPreparedToPlay) { updatableOnPreparedToPlay.Add(soundProperty); }
                    if (soundProperty.RandomizeOnSoundPlay) { updatableOnPlay.Add(soundProperty); }
                    if (soundProperty.RandomizeOnChildPlay) { updatableOnChildPlay.Add(soundProperty); }
                    if (soundProperty.ContinuousUpdate) { continuous.Add(soundProperty); }
                }

                _propertiesBySetType.Add(SetValuesType.OnPreparedToPlay, updatableOnPreparedToPlay.ToArray());
                _propertiesBySetType.Add(SetValuesType.OnBeforePlay, updatableOnPlay.ToArray());
                _propertiesBySetType.Add(SetValuesType.OnBeforeChildPlay, updatableOnChildPlay.ToArray());
                _propertiesBySetType.Add(SetValuesType.OnUpdate, continuous.ToArray());   
            }

            _initComplete = true;
        }
        
        [NonSerialized] private bool _initComplete;
    }
}
