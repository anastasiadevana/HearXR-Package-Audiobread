using UnityEngine;

namespace HearXR.Audiobread
{
    //[CreateAssetMenu (fileName = "BuiltInSoundEventSet", menuName = "Audiobread/Built In Sound Event Set")]
    public class BuiltInSoundEventSet : ScriptableObject
    {
        public BuiltInSoundEvent onEnable;
        public BuiltInSoundEvent onDisable;
    }
}
