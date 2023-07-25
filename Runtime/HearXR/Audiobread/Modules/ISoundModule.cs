namespace HearXR.Audiobread
{
    public interface ISoundModule
    {
        #region Properties
        string DisplayName { get; }
        #endregion
        
        #region Methods
        bool IsCompatibleWith(in ISoundDefinition soundDefinition);
        #endregion
        
        #region Methods
        SoundModuleProcessor CreateSoundModuleProcessor(ISound sound);
        #endregion
    }

    public interface ISoundModule<TDefinition, TProcessor> : ISoundModule 
        where TDefinition : ISoundModuleDefinition
        where TProcessor : ISoundModuleProcessor
    {
        
    }
}