namespace HearXR.Audiobread.SoundProperties
{
    public abstract class BoolSoundProperty : SoundProperty<int>
    {
        public override Calculator.CalculationMethod CalculationMethod { get; } =
            Calculator.CalculationMethod.Override;

        public override ValueContainer<int> CreateValueContainer()
        {
            return new BoolValueContainer(DefaultValue);
        }

        public override Calculator CreateCalculator()
        {
            return new BoolCalculator(this);
        }
    }
}