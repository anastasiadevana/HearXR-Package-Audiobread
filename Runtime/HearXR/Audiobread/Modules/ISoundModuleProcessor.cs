namespace HearXR.Audiobread
{
    public interface ISoundModuleProcessor
    {
        #region Properties
        ISound MySound { get; }
        bool Bypass { get; }
        #endregion
        
        #region Methods
        #endregion
    }
}