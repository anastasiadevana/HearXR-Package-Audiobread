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
    }
}