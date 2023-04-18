using System;
using HearXR.Audiobread.SoundProperties;
using UnityEngine;

namespace HearXR.Audiobread
{
    [Serializable] public class FrequencyDefinition : FloatDefinition<Frequency> {}
    
    [CreateAssetMenu (menuName = "Audiobread/Sound Property/Frequency")]
    public class Frequency : FloatSoundProperty
    {
        public override string ShortName { get; } = "Freq";
        public override float DefaultValue { get; } = 440.0f;
    
        public override bool HasMinLimit { get; } = true;
        public override float MinLimit { get; } = 10.0f;
    
        public override bool HasMaxLimit { get; } = true;
        public override float MaxLimit { get; } = 22000.0f;
        public override bool ActiveByDefault { get; } = true;
        public override bool Randomizable { get; } = true;
        public override bool SetValuesOnPreparedToPlay { get; } = true;
        public override bool RandomizeOnSoundPlay { get; } = false;
        public override bool RandomizeOnChildPlay { get; } = false;
        public override bool ContinuousUpdate { get; } = true;
        public override CalculationMethod CalculationMethod { get; } = CalculationMethod.Override;
        public override bool InfluenceChildNodes { get; } = false;
    }
}