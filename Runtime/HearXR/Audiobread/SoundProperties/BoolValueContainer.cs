using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    public class BoolValueContainer : ValueContainer<int>
    {
        public BoolValueContainer(int defaultValue) : base(defaultValue) {}

        // TODO: Can move all of these errors one level up, then override just the correct response in the inheritor class.
        public override double DoubleValue
        {
            get
            {
                Debug.LogError("Unable to get double value from an enum property.");
                return default;
            }
            set
                => Debug.LogError("Unable to set double value on an enum property.");
        }
        public override float FloatValue
        {
            get
            {
                Debug.LogError("Unable to get float value from an enum property.");
                return default;
            }
            set => Debug.LogError("Unable to set float value on an enum property.");
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
        
        public override int Value
        {
            get => IntValue;
            set => IntValue = value;
        }
    }
}