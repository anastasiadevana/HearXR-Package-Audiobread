using System.Collections.Generic;

namespace HearXR.Audiobread
{
    public interface ISoundGeneratorWrapper : ISound
    {
        List<ISoundGenerator> Generators { get; }
    }
}