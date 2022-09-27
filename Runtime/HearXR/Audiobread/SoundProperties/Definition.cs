namespace HearXR.Audiobread.SoundProperties
{
    public abstract class Definition {} // TODO: Rename this. While it's clean, it's confusing to see in other parts of the code.

    public abstract class Definition<T> : Definition {}
    
    public interface IDefinition<out T, out TProperty> where TProperty : SoundProperty<T>
    {
        TProperty SoundProperty { get; }
        
        T Value { get; }
    }
    
    [System.Serializable]
    public abstract class Definition<T, TProperty> : Definition<T>, IDefinition<T, TProperty> where TProperty : SoundProperty<T>
    {
        public T value;
        
        public bool randomize;
        
        public T variance;
        
        public TProperty soundProperty;

        public TProperty SoundProperty => soundProperty;
        
        public T Value { get; }
    }
}