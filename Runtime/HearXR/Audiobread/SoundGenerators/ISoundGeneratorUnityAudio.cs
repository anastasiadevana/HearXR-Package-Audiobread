using System;

namespace HearXR.Audiobread
{
    public interface ISoundGeneratorUnityAudio : ISoundGenerator
    {
        event Action<ISoundGeneratorUnityAudio, double> OnStealRequested;
    }
}