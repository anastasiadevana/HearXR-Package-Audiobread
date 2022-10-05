using System;
using System.Collections.Generic;
using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    public class DoubleCalculator : Calculator<double, DoubleSoundProperty, DoubleDefinition>
    {
        public DoubleCalculator(DoubleSoundProperty soundProperty) : base(soundProperty) {}

        public override void Calculate()
        {
            if (!Active) return;
            
            // TODO: Pulling straight from the definition here! Not sure if this is what we want.
            _value = _definition.value + _randomizedOffset;
            if (_property.CalculationMethod != CalculationMethod.Override)
            {
                for (int i = 0; i < _influences.Count; ++i)
                {
                    switch (_property.CalculationMethod)
                    {
                        case CalculationMethod.Multiplication:
                            _value *= _influences[i].Value;
                            break;
                        
                        case CalculationMethod.Addition:
                            _value += _influences[i].Value;
                            break;
                        
                        default:
                            Debug.LogError($"HEAR XR: Calculation method {_property.CalculationMethod} is not supported.");
                            break;
                    }
                }   
            }
            
            Audiobread.ClampDouble(_value, _property.MinLimit, _property.MaxLimit);
            
            _valueContainer.DoubleValue = _value;
        }
        
        public override void Calculate(ref Dictionary<Parameter, float> parameterValues)
        {
            if (!Active) return;
            
            // TODO: Add parameters here!
            
            Calculate();
        }

        public override void Generate()
        {
            if (!Active) return;
            
            _baseValue = _definition.value;
            
            // TODO: Maybe GetClampedRandomValue function can be in this class now, or into some Math helper class.
            // TODO: Besides the Audiobread implementation forgot about the random checkbox (if it's not checked)
            if (!_definition.randomize || !_property.Randomizable)
            {
                _rawValue = _baseValue;
                return;
            }
            
            double randomValue = (double) Audiobread.GetClampedRandomValue(_definition.value, _definition.variance, 
                _property.MinLimit, _property.MaxLimit);

            _randomizedOffset = _baseValue - randomValue;
            _rawValue = _baseValue + _randomizedOffset;

            // Debug.Log($"Generated random value for {_definition.soundProperty.name}: {randomValue} BASE {_baseValue} RANDOMIZED OFFSET {_randomizedOffset}");
        }
    }
}
