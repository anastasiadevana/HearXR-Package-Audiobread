using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    public interface IBoolDefinition<TProperty> : IDefinition<int, TProperty> where TProperty : BoolSoundProperty {}

    public interface IBoolDefinition : IBoolDefinition<BoolSoundProperty> {}

    [System.Serializable]
    public abstract class BoolDefinition : Definition<int, BoolSoundProperty>, IBoolDefinition {}

    [System.Serializable]
    public abstract class BoolDefinition<TProperty> : BoolDefinition where TProperty : BoolSoundProperty
    {
        private static BoolSoundProperty _soundProperty;

        public override BoolSoundProperty SoundProperty
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
            Debug.LogWarning("Unable to set float value on a bool sound property definition");
        }

        public override void SetBoolValue(int newValue)
        {
            value = newValue;
        }

        public override void SetDoubleValue(double newValue)
        {
            Debug.LogWarning("Unable to set double value on a bool sound property definition");
        }

        public override void SetIntValue(int newValue)
        {
            Debug.LogWarning("Unable to set int value on a bool sound property definition");
        }
    }
}