namespace HearXR.Audiobread
{
    public class ToneGeneratorSettings
    {
        public float frequency;
        public WaveShapeEnum waveShape;
    }
    
    public interface IToneGeneratorSound
    {
        ToneGeneratorSettings Settings { get; set; }
    }
}