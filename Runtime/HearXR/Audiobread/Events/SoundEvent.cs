using System.Collections.Generic;
using UnityEngine;

namespace HearXR.Audiobread
{
    public abstract class SoundEvent : ScriptableObject
    {
        [HideInInspector] public List<SoundEventReceiver> receivers = new List<SoundEventReceiver>();
        
        public void AddReceiver(SoundEventReceiver receiver)
        {
            if (!receivers.Contains(receiver))
            {
                receivers.Add(receiver);
            }
        }

        public void RemoveReceiver(SoundEventReceiver receiver)
        {
            receivers.Remove(receiver);
        }

        public void Raise(GameObject caller, GameObject other = null, SoundTriggerTag soundTriggerTag = null)
        {
            for (int i = receivers.Count - 1; i >= 0; --i)
            {
                receivers[i].OnEventRaised(this, caller, soundTriggerTag, other);
            }
        }
    }
}

