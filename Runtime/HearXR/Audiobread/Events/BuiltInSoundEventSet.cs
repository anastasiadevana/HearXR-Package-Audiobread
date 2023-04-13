using UnityEngine;

namespace HearXR.Audiobread
{
    // TODO: Handle this the same way as we handle Sound Properties.
    //[CreateAssetMenu (fileName = "BuiltInSoundEventSet", menuName = "Audiobread/Built In Sound Event Set")]
    public class BuiltInSoundEventSet : ScriptableObject
    {
        public BuiltInSoundEvent onEnable;
        public BuiltInSoundEvent onDisable;
        public BuiltInSoundEvent onUpdate;
    }
}
