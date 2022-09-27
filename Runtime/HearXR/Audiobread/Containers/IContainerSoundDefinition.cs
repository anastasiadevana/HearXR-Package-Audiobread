namespace HearXR.Audiobread
{
    public interface IContainerSoundDefinition : ISoundDefinition
    {
        ISoundDefinition[] GetChildren();
        
        int ChildCount { get; }
        
        int GetNextChildIndex(int lastChildIndex = -1);
        
        // int DefinitionSharedLastIndex { get; set; }
        
        SoundType SoundType { get; }
    }
}