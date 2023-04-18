using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    [System.Serializable] public class TimeBetweenDefinition : FloatDefinition<TimeBetween> {}
    
    [CreateAssetMenu (menuName = "Audiobread/Sound Property/Time Between Property")]
    public class TimeBetween : FloatSoundProperty
    {
        public override string ShortName { get; } = "Tim";
        public override float DefaultValue { get; } = 0.0f;
        
        public override bool HasMinLimit { get; } = true;
        public override float MinLimit { get; } = 0.0f;
        public override bool HasMaxLimit { get; } = true;
        public override float MaxLimit { get; } = 360.0f;
        
        public override bool ActiveByDefault { get; } = false;
        public override bool SetValuesOnPreparedToPlay { get; } = false;
        
        public override bool Randomizable { get; } = true;
        public override bool RandomizeOnSoundPlay { get; } = true;
        public override bool RandomizeOnChildPlay { get; } = true;
        public override bool ContinuousUpdate { get; } = false;
        
        public override CalculationMethod CalculationMethod { get; } = CalculationMethod.Addition;
        public override bool InfluenceChildNodes { get; } = false;
    }
}
