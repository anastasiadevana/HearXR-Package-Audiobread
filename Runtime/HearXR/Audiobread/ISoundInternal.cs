using System.Collections.Generic;

namespace HearXR.Audiobread
{
    internal interface ISoundInternal : ISound
    {
        void DeInit();

        // TODO: Maybe this should just be public?
        void PrepareToPlay();
        
        void InvokeOnSetParent(ISound parentSound);
        
        //List<SoundModuleProcessor> SoundModuleProcessors { get; }
        
        SoundModuleGroupProcessor SoundModuleGroupProcessor { get; }

        // bool TryGetMatchingSoundModuleProcessor(in SoundModule referenceSoundModule, out SoundModuleProcessor matchingSoundModuleProcessor);
    }
    
    internal interface ISoundInternal<in TDefinition> : ISoundInternal where TDefinition : ISoundDefinition
    {
        void Init(TDefinition soundDefinition, InitSoundFlags initFlags = InitSoundFlags.None);
    }
}