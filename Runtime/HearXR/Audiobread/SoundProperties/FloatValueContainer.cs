using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    public class FloatValueContainer : ValueContainer<float>
    {
        public FloatValueContainer(float defaultValue) : base(defaultValue) {}

        public override float Value
        {
            get => FloatValue;
            set => FloatValue = value;
        }

        public override float FloatValue
        {
            get => (IsSet) ? PropertyValue : DefaultPropertyValue;
            set
            {
                IsSet = true;
                PropertyValue = value;
            }
        }

        public override int IntValue
        {
            get
            {
                Debug.LogError("Unable to get int value from a float property.");
                return default;
            }
            set => Debug.LogError("Unable to set int value on a float property.");
        }
    }
}
