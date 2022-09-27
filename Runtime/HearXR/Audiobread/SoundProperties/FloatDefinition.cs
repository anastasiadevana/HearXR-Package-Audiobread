namespace HearXR.Audiobread.SoundProperties
{
    // TODO: Turn this into abstract?
    
    public interface IFloatDefinition<TProperty> : IDefinition<float, TProperty> where TProperty : FloatSoundProperty {}

    public interface IFloatDefinition : IFloatDefinition<FloatSoundProperty> {}
    
    [System.Serializable]
    public class FloatDefinition : Definition<float, FloatSoundProperty>, IFloatDefinition {}

    [System.Serializable]
    public class FloatDefinition<TProperty> : FloatDefinition where TProperty : FloatSoundProperty {}
}
