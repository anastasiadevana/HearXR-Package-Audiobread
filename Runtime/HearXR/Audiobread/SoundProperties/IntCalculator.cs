using System;
using System.Collections.Generic;
using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    public class IntCalculator : Calculator<int, IntSoundProperty, IntDefinition>
    {
        public IntCalculator(IntSoundProperty soundProperty) : base(soundProperty) {}

        protected override void Calculate()
        {
            if (!Active) return;
            
            _value = _rawValue;
            for (int i = 0; i < _influences.Count; ++i)
            {
                _value += _influences[i].Value;
            }
            _valueContainer.IntValue = _value;
            //Debug.Log($"Setting INT value to {_valueContainer.IntValue}");
        }
        
        public override void Calculate(ref Dictionary<Parameter, float> parameterValues)
        {
            if (!Active) return;
            
            // TODO: Incorporate parameter values calculation.
            
            Calculate();
        }

        public override void Generate()
        {
            if (!Active) return;
            
            // TODO: Maybe GetClampedRandomValue function can be in this class now, or into some Math helper class.
            // TODO: Besides the Audiobread implementation forgot about the random checkbox (if it's not checked)
            if (!_definition.randomize || !_property.Randomizable)
            {
                _rawValue = _definition.value;
                return;
            }
            _rawValue = Audiobread.GetClampedRandomValue(_definition.value, _definition.variance, _property.MinLimit, _property.MaxLimit);
        }
    }
}
