using System.Collections.Generic;

namespace HearXR.Audiobread
{
    public interface ISoundInternal : ISound
    {
        void DeInit();

        // TODO: Maybe this should just be public?
        void PrepareToPlay();
        
        void InvokeOnSetParent(ISound parentSound);
        
        //List<SoundModuleProcessor> SoundModuleProcessors { get; }
        
        SoundModuleGroupProcessor SoundModuleGroupProcessor { get; }
        
        Dictionary<Parameter, float> ParameterValues { get; }

        void AddTrackedParameters(List<Parameter> parameters);
    }
    
    public interface ISoundInternal<in TDefinition> : ISoundInternal where TDefinition : ISoundDefinition
    {
        void Init(TDefinition soundDefinition, InitSoundFlags initFlags = InitSoundFlags.None);
    }
}