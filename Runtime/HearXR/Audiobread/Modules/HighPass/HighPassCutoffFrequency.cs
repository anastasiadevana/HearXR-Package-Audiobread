using HearXR.Audiobread.SoundProperties;
using UnityEngine;

namespace HearXR.Audiobread
{
    [System.Serializable] public class HighPassCutoffFrequencyDefinition : FloatDefinition<HighPassCutoffFrequency> {}
    
    [CreateAssetMenu (menuName = "Audiobread/Sound Property/High Pass Cutoff Frequency")]
    public class HighPassCutoffFrequency : FloatSoundProperty
    {
        // public override Type SoundModuleType { get; } = typeof(HighPassSoundModule);
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
        public override bool RandomizeOnChildPlay { get; } = false;
        public override bool ContinuousUpdate { get; } = true;
        
        public override Calculator.CalculationMethod CalculationMethod { get; } = Calculator.CalculationMethod.PickSmallest;
        public override bool InfluenceChildNodes { get; } = false;
    }
}