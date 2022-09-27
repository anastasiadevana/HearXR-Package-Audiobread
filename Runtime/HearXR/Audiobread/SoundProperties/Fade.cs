namespace HearXR.Audiobread.SoundProperties
{
    /// <summary>
    /// Fade definition.
    /// </summary>
    [System.Serializable]
    public class Fade : FloatTransition
    {
        public enum Direction
        {
            None,
            In,
            Out
        }

        public Direction direction;
    }
}
