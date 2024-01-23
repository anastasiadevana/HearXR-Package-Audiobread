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

        List<SoundModuleDefinition> ModuleSoundDefinitions { get; } // TODO: Rename this to SoundModuleDefinitions
        
        /// <summary>
        /// These sound module definitions get inherited from a parent at runtime.
        /// We're keeping it in a separate variable so that it doesn't get saved to the SoundDefinition ScriptableObject.
        /// </summary>
        List<SoundModuleDefinition> RuntimeSoundModuleDefinitions { get; }
        Dictionary<SoundModule, SoundModuleDefinition> PropagatedSoundModuleDefinitions { get; set; }

        float ChanceToPlay { get; }

        string SoundDesignerNotes { get; }

        List<SoundModule> GetCompatibleModules();

        List<SoundModule> EnabledModules { get; }
                
        bool Pitched { get; }
        
        int BaseNoteNumber { get; }
    }
}