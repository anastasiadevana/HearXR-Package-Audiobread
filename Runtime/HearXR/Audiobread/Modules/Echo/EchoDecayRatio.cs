using System;
using HearXR.Audiobread.SoundProperties;
using UnityEngine;

namespace HearXR.Audiobread
{
    [System.Serializable] public class EchoDecayRatioDefinition : FloatDefinition<EchoDecayRatio> {}
    
    [CreateAssetMenu (menuName = "Audiobread/Sound Property/Echo Decay Ratio")]
    public class EchoDecayRatio : FloatSoundProperty
    {
        public override string ShortName { get; } = "Dcay";
        public override float DefaultValue { get; } = 0.5f;
    
        public override bool HasMinLimit { get; } = true;
        public override float MinLimit { get; } = 0.0f;
    
        public override bool HasMaxLimit { get; } = true;
        public override float MaxLimit { get; } = 1.0f;

        public override bool ActiveByDefault { get; } = true;
        public override bool SetValuesOnPreparedToPlay { get; } = true;
        
        public override bool Randomizable { get; } = true;
        public override bool RandomizeOnSoundPlay { get; } = true;
        public override bool RandomizeOnChildPlay { get; } = false;
        public override bool ContinuousUpdate { get; } = true;
        
        public override CalculationMethod CalculationMethod { get; } = CalculationMethod.Override;
        public override bool InfluenceChildNodes { get; } = false;
    }
}