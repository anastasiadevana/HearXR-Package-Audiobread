namespace HearXR.Audiobread.SoundProperties
{
    /// <summary>
    /// This sound controls the transition of a float sound property from one value to another.
    /// </summary>
    public abstract class FloatTransition
    {
        public FloatSoundProperty soundProperty;
        public float duration;
    }
}