namespace HearXR.Audiobread.SoundProperties
{
    public abstract class FloatSoundProperty : SoundProperty<float>
    {
        public override CalculationMethod CalculationMethod { get; } = CalculationMethod.Multiplication;

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
