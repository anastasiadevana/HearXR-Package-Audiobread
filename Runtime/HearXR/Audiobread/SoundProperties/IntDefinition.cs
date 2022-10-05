namespace HearXR.Audiobread.SoundProperties
{
    // TODO: Turn this into abstract?
    
    public interface IIntDefinition<TProperty> : IDefinition<int, TProperty> where TProperty : IntSoundProperty {}

    public interface IIntDefinition : IIntDefinition<IntSoundProperty> {}
    
    [System.Serializable]
    public abstract class IntDefinition : Definition<int, IntSoundProperty>, IIntDefinition {}

    [System.Serializable]
    public abstract class IntDefinition<TProperty> : IntDefinition where TProperty : IntSoundProperty
    {
        private static IntSoundProperty _soundProperty;
        
        public override IntSoundProperty SoundProperty
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

