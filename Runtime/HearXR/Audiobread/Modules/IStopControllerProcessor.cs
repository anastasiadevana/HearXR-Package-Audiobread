namespace HearXR.Audiobread
{
    public delegate void StopControllerStopCallback();
    
    // TODO: All the interfaces should be applied on EITHER modules OR processors. Right now we have both.
    public interface IStopControllerProcessor
    {
        bool CanStopNow(StopSoundFlags stopSoundFlags);

        void RequestStop(StopSoundFlags stopSoundFlags, StopControllerStopCallback stopCallback);
    }
}