using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    [System.Serializable]
    public class TimeDurationDefinition : FloatDefinition<TimeDuration>
    {
        public bool keepDefault;
    }
    
    [CreateAssetMenu (menuName = "Audiobread/Sound Property/Time Duration Property")]
    public class TimeDuration : FloatSoundProperty
    {
        public override string ShortName { get; } = "Dur";
        public override float DefaultValue { get; } = 1.0f;
        
        public override bool HasMinLimit { get; } = true;
        public override float MinLimit { get; } = 0.01f;
        
        public override bool HasMaxLimit { get; } = true;
        public override float MaxLimit { get; } = 1800f;
        public override bool Randomizable { get; } = true;
        
        public override Calculator.CalculationMethod CalculationMethod { get; } = Calculator.CalculationMethod.Override;
        public override bool InfluenceChildNodes { get; } = false;

        public override bool RandomizeOnSoundPlay { get; } = true;
        
        public override bool RandomizeOnChildPlay { get; } = false;
        
        public override bool ContinuousUpdate { get; } = false;
    }
}