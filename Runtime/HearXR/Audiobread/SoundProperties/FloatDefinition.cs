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
    }
}
