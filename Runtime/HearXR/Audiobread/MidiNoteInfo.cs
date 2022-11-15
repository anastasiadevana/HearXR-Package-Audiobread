using System;

namespace HearXR.Audiobread
{
    [Serializable]
    public class MidiNoteInfo
    {
        public byte noteNumber;
        public float normalizedVelocity;
        public double startTime;
        public double duration;
    }
}