using System.Collections.Generic;
using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    public class EnumCalculator : Calculator<int, EnumSoundProperty, EnumDefinition>
    {
        public EnumCalculator(EnumSoundProperty soundProperty) : base(soundProperty) {}

        protected override void Calculate()
        {
            if (!Active) return;
            _value = _rawValue;
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
            
            _rawValue = Random.Range(0, _definition.NumItems);
        }
    }
}