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
    }
}
