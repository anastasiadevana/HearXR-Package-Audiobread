using System;
using HearXR.Audiobread.SoundProperties;
using UnityEngine;

namespace HearXR.Audiobread
{
    [Serializable] public class EchoWetMixDefinition : FloatDefinition<EchoWetMix> {}
    
    [CreateAssetMenu (menuName = "Audiobread/Sound Property/Echo Wet Mix")]
    public class EchoWetMix : FloatSoundProperty
    {
        public override string ShortName { get; } = "WetMx";
        public override float DefaultValue { get; } = 1.0f;
    
        public override bool HasMinLimit { get; } = true;
        public override float MinLimit { get; } = 0.0f;
    
        public override bool HasMaxLimit { get; } = true;
        public override float MaxLimit { get; } = 1.0f;

        public override bool ActiveByDefault { get; } = true;
        public override bool SetValuesOnPreparedToPlay { get; } = true;
        
        public override bool Randomizable { get; } = true;
        public override bool RandomizeOnSoundPlay { get; } = true;
        public override bool RandomizeOnChildPlay { get; } = true;
        public override bool ContinuousUpdate { get; } = true;
        
        public override CalculationMethod CalculationMethod { get; } = CalculationMethod.Override;
        public override bool InfluenceChildNodes { get; } = false;
    }
}