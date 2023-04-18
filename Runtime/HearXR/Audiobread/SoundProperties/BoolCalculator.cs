using UnityEngine;
using System.Collections.Generic;

namespace HearXR.Audiobread.SoundProperties
{
    public class BoolCalculator : Calculator<int, BoolSoundProperty, BoolDefinition>
    {
        public BoolCalculator(BoolSoundProperty soundProperty) : base(soundProperty) {}

        protected override void Calculate()
        {
            if (!Active) return;
            
            _value = _rawValue;
            for (var i = 0; i < _influences.Count; ++i)
            {
                // TODO: Use XOR and such as for bool calculation.
                _value = _influences[i].Value;
            }

            if (_parameterOverride != null)
            {
                _value = (int) _parameterOverride;
            }

            _valueContainer.IntValue = _value;
        }
        
        public override void Calculate(ref Dictionary<Parameter, float> parameterValues)
        {
            if (!Active) return;
            
            _parameterFactor = 1.0f;
            for (var i = 0; i < _parameterArray.Length; ++i)
            {
                if (!parameterValues.ContainsKey(_parameterArray[i].parameter))
                {
                    // TODO: Barf better...
                    continue;
                }

                var value = _parameterArray[i].GetSoundPropertyValue(parameterValues[_parameterArray[i].parameter]);
                
                // TODO: Support other methods as well.
                // TODO: Treat parameters just like another influence. Everything will be more streamlined.
                switch (_parameterArray[i].CalculationMethod)
                {
                    case CalculationMethod.Override:
                        _parameterOverride = value;
                        break;
                    
                    default:
                        Debug.LogError($"HEAR XR: Calculation method for {_property} {_property.CalculationMethod} is not supported.");
                        break;
                }
            }
            
            Calculate();
        }

        public override void Generate()
        {
            if (!Active) return;
            
            if (!_definition.randomize || !_property.Randomizable)
            {
                _rawValue = _definition.value;
                return;
            }

            _rawValue = UnityEngine.Random.Range(0, 2);
        }
    }
}