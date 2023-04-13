using System;
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

            _valueContainer.IntValue = _value;
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
            
            if (!_definition.randomize || !_property.Randomizable)
            {
                _rawValue = _definition.value;
                return;
            }

            _rawValue = UnityEngine.Random.Range(0, 2);
        }
    }
}