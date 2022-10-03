using HearXR.Audiobread.Core;
using UnityEngine;

// TODO: Pitch range can be wider than -3 / 3
// TODO: Make UI more user-friendly and understandable
// TODO: Either break it up into playback speed (times) vs pitch (cents)
// TODO: Add "inverse" button for negative values
namespace HearXR.Audiobread.SoundProperties
{
    [System.Serializable] public class PitchDefinition : FloatDefinition<Pitch> {}
    
    [CreateAssetMenu (menuName = "Audiobread/Sound Property/Pitch Property")]
    public class Pitch : FloatSoundProperty
    {
        // public override System.Type SoundModuleType { get; } = typeof(CoreUnitySoundModule);
        public override string ShortName { get; } = "Pit";
        public override float DefaultValue { get; } = 1.0f;
        
        public override bool HasMinLimit { get; } = true;
        public override float MinLimit { get; } = -3.0f;
        public override bool HasMaxLimit { get; } = true;
        public override float MaxLimit { get; } = 3.0f;
        
        public override bool Randomizable { get; } = true;
        public override bool RandomizeOnSoundPlay { get; } = true;
        public override bool RandomizeOnChildPlay { get; } = true;
        public override bool ContinuousUpdate { get; } = true;
        
        public override Calculator.CalculationMethod CalculationMethod { get; } = Calculator.CalculationMethod.Multiplication;
        public override bool InfluenceChildNodes { get; } = true;
    }
}
