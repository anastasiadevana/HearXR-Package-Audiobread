using System;
using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    public class EnumValueContainer : ValueContainer<int>
    {
        public EnumValueContainer(int defaultValue) : base(defaultValue) {}

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
        
        
        
        /*
        public static T ParseEnum<T>(string value, T defaultValue) where T : struct, IConvertible
        {
            //if (!typeof(T).IsEnum) throw new ArgumentException("T must be an enumerated type");
            if (string.IsNullOrEmpty(value)) return defaultValue;

            foreach (T item in Enum.GetValues(typeof(T)))
            {
                if (item.ToString().ToLower().Equals(value.Trim().ToLower())) return item;
            }
            return defaultValue;
        }
        */
    }
}