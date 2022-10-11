namespace HearXR.Audiobread
{
    public interface ITimeBeforeListener
    {
        void HandleTimeRemainingEvent(double timeRemaining);
    }
}