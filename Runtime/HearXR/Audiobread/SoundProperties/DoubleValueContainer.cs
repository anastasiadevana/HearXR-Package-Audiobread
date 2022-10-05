using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    public class DoubleValueContainer : ValueContainer<double>
    {
        public DoubleValueContainer(double defaultValue) : base(defaultValue) {}

        public override double Value
        {
            get => DoubleValue;
            set => DoubleValue = value;
        }

        public override double DoubleValue
        {
            get => (IsSet) ? PropertyValue : DefaultPropertyValue;
            set
            {
                IsSet = true;
                PropertyValue = value;
            }
        }

        public override float FloatValue
        {
            get
            {
                Debug.LogError("Unable to get float value from a double property.");
                return default;
            }
            set => Debug.LogError("Unable to set float value on a double property.");
        }
        
        public override int IntValue
        {
            get
            {
                Debug.LogError("Unable to get int value from a double property.");
                return default;
            }
            set => Debug.LogError("Unable to set int value on a double property.");
        }
    }
}
