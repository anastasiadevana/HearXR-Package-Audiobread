using System;
using HearXR.Audiobread.SoundProperties;
using UnityEngine;

namespace HearXR.Audiobread
{
    [System.Serializable] public class LowPassCutoffFrequencyDefinition : FloatDefinition<LowPassCutoffFrequency> {}
    
    [CreateAssetMenu (menuName = "Audiobread/Sound Property/Low Pass Cutoff Frequency")]
    public class LowPassCutoffFrequency : FloatSoundProperty
    {
        // public override Type SoundModuleType { get; } = typeof(LowPassSoundModule);
        public override string ShortName { get; } = "Freq";
        public override float DefaultValue { get; } = 5000.0f;
    
        public override bool HasMinLimit { get; } = true;
        public override float MinLimit { get; } = 10.0f;
    
        public override bool HasMaxLimit { get; } = true;
        public override float MaxLimit { get; } = 22000.0f;
        
        
        public override bool ActiveByDefault { get; } = true;
        public override bool SetValuesOnPreparedToPlay { get; } = false;
        
        public override bool Randomizable { get; } = true;
        public override bool RandomizeOnSoundPlay { get; } = true;
        public override bool RandomizeOnChildPlay { get; } = true;
        public override bool ContinuousUpdate { get; } = true;
        
        public override Calculator.CalculationMethod CalculationMethod { get; } = Calculator.CalculationMethod.PickSmallest;
        public override bool InfluenceChildNodes { get; } = false;
    }
}