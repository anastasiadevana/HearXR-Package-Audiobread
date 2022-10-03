namespace HearXR.Audiobread.SoundProperties
{
    public abstract class FloatSoundProperty : SoundProperty<float>
    {
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
