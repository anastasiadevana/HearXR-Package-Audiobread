using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    [System.Serializable] public class MusicalLengthDefinition : IntDefinition<MusicalLength> {}
    
    [CreateAssetMenu (menuName = "Audiobread/Musical Length Property")]
    public class MusicalLength : IntSoundProperty
    {
        // public override System.Type SoundModuleType { get; } = typeof(SoundModule); // TODO:
        public override string ShortName { get; } = "Lgth";
        public override int DefaultValue { get; } = 0;
        
        public override bool HasMinLimit { get; } = true;
        public override int MinLimit { get; } = 0;
        
        public override bool HasMaxLimit { get; } = true;
        public override int MaxLimit { get; } = 100;
        public override bool Randomizable { get; } = true;

        public override bool RandomizeOnSoundPlay { get; } = true;

        public override bool RandomizeOnChildPlay { get; } = false;

        public override bool ContinuousUpdate { get; } = false;

        public override Calculator.CalculationMethod CalculationMethod { get; } = Calculator.CalculationMethod.Override;
        public override bool InfluenceChildNodes { get; } = false;
    }
}
