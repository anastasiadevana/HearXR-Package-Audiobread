using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace HearXR.Audiobread
{
    public class SoundEventTrigger : MonoBehaviour
    {
        public SoundTriggerTag soundTriggerTag;

        [System.Serializable]
        public class SoundEventList
        {
            public BuiltInSoundEvent soundEvent;
            public CustomSoundEvent replaceWithCustomSoundEvent;
            public GameObject overrideTriggerObject;
            public GameObject overrideOtherObject;
            public SoundTriggerTag overrideSoundTriggerTag;
        }

        // Validate if these are false against the sound event list.
        [SerializeField] private bool _raiseOnEnable = true;
        [SerializeField] private bool _raiseOnDisable = true;
        [FormerlySerializedAs("_soundEventList")] [SerializeField] private List<SoundEventList> _soundEventOverrides = new List<SoundEventList>();
        
        private List<BuiltInSoundEvent> _builtInSoundEventList = new List<BuiltInSoundEvent>();

        private void Awake()
        {
            for (int i = 0; i < _soundEventOverrides.Count; i++)
            {
                if (!_builtInSoundEventList.Contains(_soundEventOverrides[i].soundEvent))
                {
                    _builtInSoundEventList.Add(_soundEventOverrides[i].soundEvent);
                }
            }
        }
        
        private void OnEnable()
        {
            
            
            
            
            
            
            
            
            TryRaiseBuiltInEvent(_raiseOnEnable, BuiltInData.Events.onEnable);
            
            
            
            
            
            
            
        }
        
        private void OnDisable()
        {
            
            
            
            
            
            
            
            
            TryRaiseBuiltInEvent(_raiseOnDisable, BuiltInData.Events.onDisable);
            
            
            
            
            
            
            
        }

        private void TryRaiseBuiltInEvent(bool doRaise, BuiltInSoundEvent soundEvent)
        {
            if (!doRaise && !_builtInSoundEventList.Contains(soundEvent))
            {
                return;
            }

            if (doRaise)
            {
                soundEvent.Raise(gameObject, null, soundTriggerTag);   
            }

            if (_builtInSoundEventList.Contains(soundEvent))
            {
                ForwardEvents(soundEvent);
            }
        }
        
        private void ForwardEvents(BuiltInSoundEvent soundEvent)
        {
            // TODO: Do this once on startup.
            for (int i = 0; i < _soundEventOverrides.Count; ++i)
            {
                if (_soundEventOverrides[i].soundEvent != soundEvent)
                {
                    continue;
                }

                SoundEvent newSoundEvent = (_soundEventOverrides[i].replaceWithCustomSoundEvent != null) ? _soundEventOverrides[i].replaceWithCustomSoundEvent as SoundEvent : soundEvent as SoundEvent;
                GameObject triggerObject = (_soundEventOverrides[i].overrideTriggerObject != null) ? _soundEventOverrides[i].overrideTriggerObject : gameObject;
                GameObject otherObject = (_soundEventOverrides[i].overrideOtherObject != null) ? _soundEventOverrides[i].overrideOtherObject : null;
                SoundTriggerTag tag = (_soundEventOverrides[i].overrideSoundTriggerTag != null) ? _soundEventOverrides[i].overrideSoundTriggerTag : soundTriggerTag;
                
                newSoundEvent.Raise(triggerObject, otherObject, tag);
            }
        }
    }   
}
