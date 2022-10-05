using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    public class IntValueContainer : ValueContainer<int>
    {
        public IntValueContainer(int defaultValue) : base(defaultValue) {}

        public override int Value
        {
            get => IntValue;
            set => IntValue = value;
        }

        public override int IntValue
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
                Debug.LogError("Unable to get float value from an int property.");
                return default;
            }
            set => Debug.LogError("Unable to set float value to an int property.");
        }
        
        public override double DoubleValue
        {
            get
            {
                Debug.LogError("Unable to get double value from an int property.");
                return default;
            }
            set => Debug.LogError("Unable to set double value on an int property.");
        }
    }
}
