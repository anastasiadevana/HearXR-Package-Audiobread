using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    [System.Serializable] public class OffsetDefinition : FloatDefinition<Offset> {}
    
    [CreateAssetMenu (menuName = "Audiobread/Sound Property/Offset Property")]
    public class Offset : FloatSoundProperty
    {
        // public override System.Type SoundModuleType { get; } = typeof(SoundModule); // TODO:
        public override string ShortName { get; } = "Ofs";
        
        public override float DefaultValue { get; } = 0.0f;
        
        public override bool HasMinLimit { get; } = true;
        public override float MinLimit { get; } = 0.0f;
        
        public override bool HasMaxLimit { get; } = true;
        public override float MaxLimit { get; } = 360.0f;
        
        public override bool ActiveByDefault { get; } = false;
        public override bool SetValuesOnPreparedToPlay { get; } = false;
        public override bool Randomizable { get; } = true;
        
        public override Calculator.CalculationMethod CalculationMethod { get; } = Calculator.CalculationMethod.Addition;
        public override bool InfluenceChildNodes { get; } = true;
        
        public override bool RandomizeOnSoundPlay { get; } = true;
        
        public override bool RandomizeOnChildPlay { get; } = true;
        
        public override bool ContinuousUpdate { get; } = false;
    }
}
