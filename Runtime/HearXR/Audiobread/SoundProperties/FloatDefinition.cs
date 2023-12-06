using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    public interface IFloatDefinition<TProperty> : IDefinition<float, TProperty> where TProperty : FloatSoundProperty {}

    public interface IFloatDefinition : IFloatDefinition<FloatSoundProperty> {}
    
    [System.Serializable]
    public abstract class FloatDefinition : Definition<float, FloatSoundProperty>, IFloatDefinition {}

    [System.Serializable]
    public abstract class FloatDefinition<TProperty> : FloatDefinition where TProperty : FloatSoundProperty
    {
        private static FloatSoundProperty _soundProperty;
        
        public override FloatSoundProperty SoundProperty
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
            value = Mathf.Clamp(newValue, SoundProperty.MinLimit, SoundProperty.MaxLimit);
        }

        public override void SetBoolValue(int newValue)
        {
            Debug.LogWarning("Unable to set bool value on a float sound property definition");
        }

        public override void SetDoubleValue(double newValue)
        {
            Debug.LogWarning("Unable to set double value on a float sound property definition");
        }

        public override void SetIntValue(int newValue)
        {
            Debug.LogWarning("Unable to set int value on a float sound property definition");
        }
    }
}
