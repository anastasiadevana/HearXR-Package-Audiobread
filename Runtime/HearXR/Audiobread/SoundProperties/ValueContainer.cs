using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    public abstract class ValueContainer
    {
        public bool IsSet
        {
            get => _isSet;
            set
            {
                _isSet = value;
                if (!_isSet) ResetToDefault();
            }
        }

        protected bool _isSet;

        public abstract double DoubleValue { get; set; }
        
        public abstract float FloatValue { get; set; }

        public abstract int IntValue { get; set; }

        public abstract void ResetToDefault();
    }
    
    public abstract class ValueContainer<T> : ValueContainer
    {
        protected ValueContainer(T defaultValue)
        {
            _isSet = false;
            _propertyValue = defaultValue;
            _defaultPropertyValue = defaultValue;
        }

        public abstract T Value { get; set; }

        protected T PropertyValue
        {
            get => _propertyValue;
            set => _propertyValue = value;
        }

        private T _propertyValue;

        protected T DefaultPropertyValue
        {
            get => _defaultPropertyValue;
            set => _defaultPropertyValue = value;
        }

        private T _defaultPropertyValue;

        public override void ResetToDefault()
        {
            _propertyValue = _defaultPropertyValue;
        }
    }
}