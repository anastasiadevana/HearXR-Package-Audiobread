using System.Collections.Generic;
using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    public class EnumCalculator : Calculator<int, EnumSoundProperty, EnumDefinition>
    {
        public EnumCalculator(EnumSoundProperty soundProperty) : base(soundProperty) {}

        public override void Calculate()
        {
            if (!Active) return;
            _value = _rawValue;
            _valueContainer.IntValue = _value;
        }
        
        public override void Calculate(ref Dictionary<Parameter, float> parameterValues)
        {
            if (!Active) return;
            Calculate();
        }

        public override void Generate()
        {
            if (!Active) return;
            _rawValue = Random.Range(0, _definition.NumItems);
        }
    }
}