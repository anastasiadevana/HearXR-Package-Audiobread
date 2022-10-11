using System;
using System.Collections.Generic;
using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    public class FloatCalculator : Calculator<float, FloatSoundProperty, FloatDefinition>
    {
        public FloatCalculator(FloatSoundProperty soundProperty) : base(soundProperty) {}

        protected override void Calculate()
        {
            if (!Active) return;
            
            var influenceFactor = 1.0f;
            var influenceAddition = 0.0f;
            
            if (_property.CalculationMethod != CalculationMethod.Override)
            {
                for (int i = 0; i < _influences.Count; ++i)
                {
                    switch (_property.CalculationMethod)
                    {
                        case CalculationMethod.Multiplication:
                            influenceFactor *= _influences[i].Value;
                            break;
                        
                        case CalculationMethod.Addition:
                            influenceAddition += _influences[i].Value;
                            break;
                        
                        default:
                            Debug.LogError($"HEAR XR: Calculation method {_property.CalculationMethod} is not supported.");
                            break;
                    }
                }   
            }

            // Adjusted value.
            // TODO: Work in parameters (move from volume)
            _value = (_rawValue + influenceAddition) * influenceFactor;
            
            Mathf.Clamp(_value, _property.MinLimit, _property.MaxLimit);
            
            _valueContainer.FloatValue = _value;
        }
        
        public override void Calculate(ref Dictionary<Parameter, float> parameterValues)
        {
            if (!Active) return;
            
            _parameterFactor = 1.0f;
            for (var i = 0; i < _parameterArray.Length; ++i)
            {
                if (!parameterValues.ContainsKey(_parameterArray[i].parameter))
                {
                    // TODO: Barf better.
                    continue;
                }

                // TODO: Are parameters calculated with different methods as well? (multiplication, addition, etc)
                _parameterFactor *= _parameterArray[i].GetSoundPropertyValue(parameterValues[_parameterArray[i].parameter]);
            }

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
            
            var randomValue = Audiobread.GetClampedRandomValue(_definition.value, _definition.variance, 
                _property.MinLimit, _property.MaxLimit);

            _randomizedOffset = _baseValue - randomValue;
            _rawValue = _baseValue + _randomizedOffset;

            // Debug.Log($"Generated random value for {_definition.soundProperty.name}: {randomValue} BASE {_baseValue} RANDOMIZED OFFSET {_randomizedOffset}");
        }
    }
}
