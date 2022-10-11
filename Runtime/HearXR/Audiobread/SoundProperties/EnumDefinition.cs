using System;

namespace HearXR.Audiobread.SoundProperties
{
    public interface IEnumDefinition<TProperty> : IDefinition<int, TProperty> where TProperty : EnumSoundProperty {}

    public interface IEnumDefinition : IEnumDefinition<EnumSoundProperty> {}

    [System.Serializable]
    public abstract class EnumDefinition : Definition<int, EnumSoundProperty>, IEnumDefinition
    {
        public abstract int NumItems { get; }
    }

    [System.Serializable]
    public abstract class EnumDefinition<TProperty, TEnum> : EnumDefinition where TProperty : EnumSoundProperty
                                                                            where TEnum : Enum
    {
        public TEnum myEnum;

        private static EnumSoundProperty _soundProperty;

        private int _numItems = -1;
        
        public override EnumSoundProperty SoundProperty
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

        public override int NumItems
        {
            get
            {
                if (_numItems < 0)
                {
                    _numItems = Enum.GetNames(typeof(TEnum)).Length;
                }

                return _numItems;
            }
        }
    }
}