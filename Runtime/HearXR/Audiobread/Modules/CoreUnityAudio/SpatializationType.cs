using System;
using HearXR.Audiobread.SoundProperties;
using UnityEngine;

namespace HearXR.Audiobread
{
    [Serializable]
    public class SpatializationTypeDefinition : EnumDefinition<SpatializationType, SpatializationTypeEnum> {}

    [CreateAssetMenu (menuName = "Audiobread/Sound Property/Spatialization Type")]
    public class SpatializationType : EnumSoundProperty
    {
        public override string ShortName { get; } = "Sptl";    
        public override int DefaultValue { get; } = 0;
    
        public override bool HasMinLimit { get; } = true;
        public override int MinLimit { get; } = 0;
    
        public override bool HasMaxLimit { get; } = true;

        public override int MaxLimit => Enum.GetNames(typeof(SpatializationTypeEnum)).Length;

        public override bool ActiveByDefault { get; } = true;
        public override bool SetValuesOnPreparedToPlay { get; } = true;
        
        public override bool Randomizable { get; } = false;
        public override bool RandomizeOnSoundPlay { get; } = false;
        public override bool RandomizeOnChildPlay { get; } = false;
        public override bool ContinuousUpdate { get; } = false;
        
        public override Calculator.CalculationMethod CalculationMethod { get; } = Calculator.CalculationMethod.Override;
        public override bool InfluenceChildNodes { get; } = false;
    }
}