using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    [System.Serializable] public class DelayDefinition : FloatDefinition<Delay> {}
    
    [CreateAssetMenu (menuName = "Audiobread/Delay Property")]
    public class Delay : FloatSoundProperty
    {
        public override string ShortName { get; } = "Del";
        
        public override float DefaultValue { get; } = 0.0f;
        
        public override bool HasMinLimit { get; } = true;
        public override float MinLimit { get; } = 0.0f;
        
        public override bool HasMaxLimit { get; } = true;
        public override float MaxLimit { get; } = 360.0f;
        public override bool Randomizable { get; } = true;
        
        public override Calculator.CalculationMethod CalculationMethod { get; } = Calculator.CalculationMethod.Addition;
        public override bool InfluenceChildNodes { get; } = false;
        
        public override bool RandomizeOnSoundPlay { get; } = true;
        
        public override bool RandomizeOnChildPlay { get; } = true;
        
        public override bool ContinuousUpdate { get; } = false;
    }
}
