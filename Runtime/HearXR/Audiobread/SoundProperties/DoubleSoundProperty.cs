﻿namespace HearXR.Audiobread.SoundProperties
{
    public abstract class DoubleSoundProperty : SoundProperty<double>
    {
        public override CalculationMethod CalculationMethod { get; } = CalculationMethod.Multiplication;

        public override ValueContainer<double> CreateValueContainer()
        {
            return new DoubleValueContainer(DefaultValue);
        }

        public override Calculator CreateCalculator()
        {
            return new DoubleCalculator(this);
        }
    }
}
