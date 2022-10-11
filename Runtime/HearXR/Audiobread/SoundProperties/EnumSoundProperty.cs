using System;

namespace HearXR.Audiobread.SoundProperties
{
    public abstract class EnumSoundProperty : SoundProperty<int>
    {
        public override Calculator.CalculationMethod CalculationMethod { get; } =
            Calculator.CalculationMethod.Override;

        public override ValueContainer<int> CreateValueContainer()
        {
            return new EnumValueContainer(DefaultValue);
        }

        public override Calculator CreateCalculator()
        {
            return new EnumCalculator(this);
        }
    }
}