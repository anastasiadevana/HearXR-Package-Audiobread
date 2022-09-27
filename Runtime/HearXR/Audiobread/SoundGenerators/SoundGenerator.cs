namespace HearXR.Audiobread
{
    public abstract class SoundGenerator<TDefinition, TSelf> : Sound<TDefinition, TSelf>, ISoundGenerator
        where TDefinition : class, ISoundDefinition
        where TSelf : Sound
    {
        
    }
}