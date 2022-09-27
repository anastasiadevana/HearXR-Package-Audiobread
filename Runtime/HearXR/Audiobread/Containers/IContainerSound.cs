namespace HearXR.Audiobread
{
    /// <summary>
    /// Sound container doesn't produce any sounds.
    /// </summary>
    public interface IContainerSound : ISound
    {
        bool IsTopParent { get; }

        ISound[] Children { get; }
    }
}
