namespace HearXR.Audiobread.SoundProperties
{
    public interface IDefinition
    {
        void SetFloatValue(float newValue);
        void SetIntValue(int newValue);
        void SetDoubleValue(double newValue);
        void SetBoolValue(int newValue);
    }
    
    // TODO: Rename this. While it's clean, it's confusing to see in other parts of the code.
    public abstract class Definition : IDefinition
    {
        public bool active = true;
        
        public abstract void SetFloatValue(float newValue);

        public abstract void SetIntValue(int newValue);

        public abstract void SetDoubleValue(double newValue);
        
        public abstract void SetBoolValue(int newValue);
    }

    public abstract class Definition<T> : Definition {}

    public interface IDefinition<out TProperty> : IDefinition where TProperty : ISoundProperty
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