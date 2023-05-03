using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    [System.Serializable]
    public class TimeDurationDefinition : DoubleDefinition<TimeDuration> {}
    
    // [CreateAssetMenu (menuName = "Audiobread/Sound Property/Time Duration Property")]
    public class TimeDuration : DoubleSoundProperty
    {
        public override string ShortName { get; } = "TDur";
        public override double DefaultValue { get; } = 1.0d;
        
        public override bool HasMinLimit { get; } = true;
        public override double MinLimit { get; } = 0.01d;
        public override bool HasMaxLimit { get; } = true;
        public override double MaxLimit { get; } = 1800d;
        
        public override bool ActiveByDefault { get; } = false;
        public override bool SetValuesOnPreparedToPlay { get; } = false;
        
        public override bool Randomizable { get; } = true;
        public override bool InfluenceChildNodes { get; } = false;
        public override bool RandomizeOnSoundPlay { get; } = true;
        public override bool RandomizeOnChildPlay { get; } = false;
        
        public override CalculationMethod CalculationMethod { get; } = CalculationMethod.Override;
        
        public override bool ContinuousUpdate { get; } = false;
    }
}