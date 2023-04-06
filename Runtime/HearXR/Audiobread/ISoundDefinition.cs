using System.Collections.Generic;

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
        
        List<SoundParameterDefinition> Parameters { get; }
        
        float ChanceToPlay { get; }

        string SoundDesignerNotes { get; }

        List<SoundModule> GetCompatibleModules();

        List<SoundModule> EnabledModules { get; }
                
        bool Pitched { get; }
        
        int BaseNoteNumber { get; }
    }
}