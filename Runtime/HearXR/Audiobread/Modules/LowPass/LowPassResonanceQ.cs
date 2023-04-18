using System;
using HearXR.Audiobread.SoundProperties;
using UnityEngine;

namespace HearXR.Audiobread
{
    [System.Serializable] public class LowPassResonanceQDefinition : FloatDefinition<LowPassResonanceQ> {}
    
    [CreateAssetMenu (menuName = "Audiobread/Sound Property/Low Pass Resonance Q")]
    public class LowPassResonanceQ : FloatSoundProperty
    {
        // public override Type SoundModuleType { get; } = typeof(LowPassSoundModule);
        public override string ShortName { get; } = "Q";
        public override float DefaultValue { get; } = 1.0f;
    
        public override bool HasMinLimit { get; } = true;
        public override float MinLimit { get; } = 1.0f;
    
        public override bool HasMaxLimit { get; } = true;
        public override float MaxLimit { get; } = 10.0f;
        
        public override bool ActiveByDefault { get; } = false;
        public override bool SetValuesOnPreparedToPlay { get; } = false;
        
        public override bool Randomizable { get; } = true;
        public override bool RandomizeOnSoundPlay { get; } = true;
        public override bool RandomizeOnChildPlay { get; } = true;
        public override bool ContinuousUpdate { get; } = true;
        
        public override CalculationMethod CalculationMethod { get; } = CalculationMethod.PickBiggest;
        public override bool InfluenceChildNodes { get; } = false;
    }
}