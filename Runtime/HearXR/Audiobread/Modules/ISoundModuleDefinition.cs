using System.Collections.Generic;
using System.Reflection;
using HearXR.Audiobread.SoundProperties;

namespace HearXR.Audiobread
{
    public interface ISoundModuleDefinition
    {
        bool TryGetSoundProperty<T>(FieldInfo fieldInfo, out T soundProperty) where T : SoundProperty;
        
        List<SoundParameterDefinition> Parameters { get; }
    }
}