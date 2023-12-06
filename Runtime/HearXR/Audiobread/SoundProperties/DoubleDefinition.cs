using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    public interface IDoubleDefinition<TProperty> : IDefinition<double, TProperty> where TProperty : DoubleSoundProperty {}

    public interface IDoubleDefinition : IDoubleDefinition<DoubleSoundProperty> {}
    
    [System.Serializable]
    public abstract class DoubleDefinition : Definition<double, DoubleSoundProperty>, IDoubleDefinition {}

    [System.Serializable]
    public abstract class DoubleDefinition<TProperty> : DoubleDefinition where TProperty : DoubleSoundProperty, ISoundProperty
    {
        private static DoubleSoundProperty _soundProperty;
        
        public override DoubleSoundProperty SoundProperty
        {
            get
            {
                if (_soundProperty == null)
                {
                    _soundProperty = BuiltInData.Properties.GetSoundPropertyByType<TProperty>();
                }
        
                return _soundProperty;
            }
        }
        
        public override void SetFloatValue(float newValue)
        {
            Debug.LogWarning("Unable to set float value on a double sound property definition");
        }

        public override void SetBoolValue(int newValue)
        {
            Debug.LogWarning("Unable to set bool value on a double sound property definition");
        }

        public override void SetDoubleValue(double newValue)
        {
            if (newValue < SoundProperty.MinLimit) newValue = SoundProperty.MinLimit;
            if (newValue > SoundProperty.MaxLimit) newValue = SoundProperty.MaxLimit;
            value = newValue;
        }

        public override void SetIntValue(int newValue)
        {
            Debug.LogWarning("Unable to set int value on a double sound property definition");
        }
    }
}
