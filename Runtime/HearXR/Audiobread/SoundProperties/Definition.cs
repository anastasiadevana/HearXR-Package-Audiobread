namespace HearXR.Audiobread.SoundProperties
{
    // TODO: Rename this. While it's clean, it's confusing to see in other parts of the code.
    public abstract class Definition
    {
        public bool active = true;
    }

    public abstract class Definition<T> : Definition {}

    public interface IDefinition<out TProperty> where TProperty : ISoundProperty
    {
        TProperty SoundProperty { get; }
    }
    
    public interface IDefinition<out T, out TProperty> : IDefinition<TProperty> where TProperty : SoundProperty<T>, ISoundProperty
    {
        T Value { get; }
    }

    [System.Serializable]
    public abstract class Definition<T, TProperty> : Definition<T>, IDefinition<T, TProperty> where TProperty : SoundProperty<T>, ISoundProperty
    {
        public T value;
        
        public bool randomize;
        
        public T variance;

        public TProperty soundProperty; // Used for storing a serialized reference.

        public abstract TProperty SoundProperty { get; }

        public T Value { get; }
    }
}