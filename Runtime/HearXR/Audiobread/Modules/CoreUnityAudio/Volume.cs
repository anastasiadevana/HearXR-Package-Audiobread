using HearXR.Audiobread.Core;
using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    [System.Serializable] public class VolumeDefinition : FloatDefinition<Volume> {}
    
    [CreateAssetMenu (menuName = "Audiobread/Sound Property/Volume Property")]
    public class Volume : FloatSoundProperty
    {
        public override string ShortName { get; } = "Vol";
        public override float DefaultValue { get; } = 1.0f;
        
        public override bool HasMinLimit { get; } = true;
        public override float MinLimit { get; } = 0.0f;
        
        public override bool HasMaxLimit { get; } = true;
        public override float MaxLimit { get; } = 1.0f;
        
        public override bool ActiveByDefault { get; } = true;
        public override bool SetValuesOnPreparedToPlay { get; } = false;
        public override bool Randomizable { get; } = true;
        
        public override Calculator.CalculationMethod CalculationMethod { get; } = Calculator.CalculationMethod.Multiplication;
        public override bool InfluenceChildNodes { get; } = true;

        public override bool RandomizeOnSoundPlay { get; } = true;
        
        public override bool RandomizeOnChildPlay { get; } = true;
        
        public override bool ContinuousUpdate { get; } = true;
        
        public override Calculator CreateCalculator()
        {
            return new VolumeCalculator(this);
        }
    }
}
