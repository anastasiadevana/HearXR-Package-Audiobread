namespace HearXR.Audiobread.SoundProperties
{
    public abstract class FloatSoundProperty : SoundProperty<float>
    {
        // public override string ShortName => shortName;
        // public override bool Randomizable => randomizable;
        // public override float DefaultValue => defaultValue;
        // public override bool HasMinLimit => hasMinLimit;
        // public override float MinLimit => minLimit;
        // public override bool HasMaxLimit => hasMaxLimit;
        // public override float MaxLimit => maxLimit;
        // public override bool RandomizeOnSoundPlay => randomizeOnSoundPlay;
        // public override bool RandomizeOnChildPlay => randomizeOnChildPlay;
        // public override bool ContinuousUpdate => continuousUpdate;
        
        
        #region Public Fields
        // public string shortName;
        // public float defaultValue;
        // public bool hasMinLimit;
        // public float minLimit;
        // public bool hasMaxLimit;
        // public float maxLimit;
        // public bool randomizable;
        // public bool randomizeOnSoundPlay = true;
        // public bool randomizeOnChildPlay = true;
        // public bool continuousUpdate;
        #endregion

        public override Calculator.CalculationMethod CalculationMethod { get; } =
            Calculator.CalculationMethod.Multiplication;

        public override ValueContainer<float> CreateValueContainer()
        {
            return new FloatValueContainer(DefaultValue);
        }

        public override Calculator CreateCalculator()
        {
            return new FloatCalculator(this);
        }
    }
}
