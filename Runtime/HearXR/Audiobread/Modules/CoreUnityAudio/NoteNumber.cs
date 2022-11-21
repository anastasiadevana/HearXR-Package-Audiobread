using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    
    [System.Serializable] public class NoteNumberDefinition : IntDefinition<NoteNumber> {}

    [CreateAssetMenu(menuName = "Audiobread/Sound Property/Note Number Property")]
    public class NoteNumber : IntSoundProperty
    {
        public override string ShortName { get; } = "Note";
        public override int DefaultValue { get; } = Audiobread.NOTE_NUMBER_C4;
    
        public override bool HasMinLimit { get; } = true;
        public override int MinLimit { get; } = 0;
    
        public override bool HasMaxLimit { get; } = true;
        public override int MaxLimit { get; } = 127;
    
        public override bool ActiveByDefault { get; } = false;
        public override bool SetValuesOnPreparedToPlay { get; } = true;
        public override bool Randomizable { get; } = true;

        public override bool RandomizeOnSoundPlay { get; } = true;

        public override bool RandomizeOnChildPlay { get; } = true;

        public override bool ContinuousUpdate { get; } = false;

        public override Calculator.CalculationMethod CalculationMethod { get; } = Calculator.CalculationMethod.Override;
        public override bool InfluenceChildNodes { get; } = true;
    }
}