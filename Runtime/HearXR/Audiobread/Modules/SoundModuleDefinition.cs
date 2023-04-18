using System;
using System.Collections.Generic;
using System.Reflection;
using HearXR.Audiobread.SoundProperties;
using UnityEngine;

namespace HearXR.Audiobread
{
    public abstract class SoundModuleDefinition : ScriptableObject, ISoundModuleDefinition
    {
        public SoundModule soundModule;
        public bool bypass;
        
        [Space(20)] public List<SoundParameterDefinition> parameters;
        
        [NonSerialized] private bool _cachedProperties;
        [NonSerialized] private Dictionary<SoundProperty, Definition> _soundProperties = new Dictionary<SoundProperty, Definition>();

        public List<SoundParameterDefinition> Parameters
        {
            get => parameters;
            set => parameters = value;
        }
        
        public Dictionary<SoundProperty, Definition> GetSoundProperties()
        {
            if (!_cachedProperties)
            {
                CacheProperties(ref _soundProperties);
                _cachedProperties = true;
            }

            return _soundProperties;
        }

        public bool TryGetSoundProperty<T>(FieldInfo fieldInfo, out T soundProperty) where T : SoundProperty
        {
            soundProperty = default;
            var found = false;
            
            if (fieldInfo.GetValue(this) is IDefinition<ISoundProperty> fieldValue)
            {
                soundProperty = fieldValue.SoundProperty as T;
                found = true;
            }

            return found;
        }
        
        protected virtual void CacheProperties(ref Dictionary<SoundProperty, Definition> soundProperties) {}
    }
}