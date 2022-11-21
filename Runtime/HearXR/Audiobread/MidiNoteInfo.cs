using System;

namespace HearXR.Audiobread
{
    [Serializable]
    public class MidiNoteInfo
    {
        public int noteNumber;
        // 0 - 1 range
        public float velocity;
        public double startTime;
        public double duration;
    }
}