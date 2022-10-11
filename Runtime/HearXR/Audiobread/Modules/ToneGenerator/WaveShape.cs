using System;
using HearXR.Audiobread.SoundProperties;
using UnityEngine;

namespace HearXR.Audiobread
{
    [Serializable]
    public class WaveShapeDefinition : EnumDefinition<WaveShape, WaveShapeEnum> {}

    [CreateAssetMenu (menuName = "Audiobread/Sound Property/Wave Shape")]
    public class WaveShape : EnumSoundProperty
    {
        public override string ShortName { get; } = "Shape";    
        public override int DefaultValue { get; } = 0;
    
        public override bool HasMinLimit { get; } = true;
        public override int MinLimit { get; } = 0;
    
        public override bool HasMaxLimit { get; } = true;

        public override int MaxLimit => Enum.GetNames(typeof(WaveShapeEnum)).Length;

        public override bool ActiveByDefault { get; } = true;
        public override bool SetValuesOnPreparedToPlay { get; } = true;
        
        public override bool Randomizable { get; } = true;
        public override bool RandomizeOnSoundPlay { get; } = false;
        public override bool RandomizeOnChildPlay { get; } = false;
        public override bool ContinuousUpdate { get; } = false;
        
        public override Calculator.CalculationMethod CalculationMethod { get; } = Calculator.CalculationMethod.Override;
        public override bool InfluenceChildNodes { get; } = false;
    }
}