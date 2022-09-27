using System.Collections.Generic;
using HearXR.Audiobread.SoundProperties;

namespace HearXR.Audiobread
{
    /// <summary>
    /// Defines any sound.
    /// </summary>
    public interface ISoundDefinition
    {
        ISound CreateSound(InitSoundFlags initSoundFlags = InitSoundFlags.None);
    
        T CreateSound<T>(InitSoundFlags initSoundFlags = InitSoundFlags.None) where T : class, ISound;

        string Name { get; }
        
        bool WasChanged { get; set; }

        List<SoundModuleDefinition> ModuleSoundDefinitions { get; }
        
        // VolumeDefinition VolumeDefinition { get; }
        
        List<SoundParameter> Parameters { get; }

        List<SoundModule> GetCompatibleModules();

        List<SoundModule> EnabledModules { get; }
    }
}