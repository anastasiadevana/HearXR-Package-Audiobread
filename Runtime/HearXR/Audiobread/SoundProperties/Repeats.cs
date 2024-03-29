﻿using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    [System.Serializable] public class RepeatsDefinition : IntDefinition<Repeats> {}
    
    // [CreateAssetMenu (menuName = "Audiobread/Sound Property/Repeats Property")]
    public class Repeats : IntSoundProperty
    {
        public override string ShortName { get; } = "Rpt";
        public override int DefaultValue { get; } = 1;
        
        public override bool HasMinLimit { get; } = true;
        public override int MinLimit { get; } = 0;
        
        public override bool HasMaxLimit { get; } = true;
        public override int MaxLimit { get; } = 100;
        
        public override bool ActiveByDefault { get; } = true;
        public override bool SetValuesOnPreparedToPlay { get; } = false;
        public override bool Randomizable { get; } = true;

        public override bool RandomizeOnSoundPlay { get; } = true;

        public override bool RandomizeOnChildPlay { get; } = false;

        public override bool ContinuousUpdate { get; } = false;

        public override CalculationMethod CalculationMethod { get; } = CalculationMethod.Override;
        public override bool InfluenceChildNodes { get; } = false;
    }
}
