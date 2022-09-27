namespace HearXR.Audiobread.SoundProperties
{
    // TODO: Turn this into abstract?
    
    public interface IIntDefinition<TProperty> : IDefinition<int, TProperty> where TProperty : IntSoundProperty {}

    public interface IIntDefinition : IIntDefinition<IntSoundProperty> {}
    
    [System.Serializable]
    public class IntDefinition : Definition<int, IntSoundProperty>, IIntDefinition {}

    [System.Serializable]
    public class IntDefinition<TProperty> : IntDefinition where TProperty : IntSoundProperty {}
}

