using System;
using System.Collections.Generic;
using UnityEngine;

namespace HearXR.Audiobread
{
    [DefaultExecutionOrder(-50)]
    [AddComponentMenu("Audiobread/Sound Event Receiver")]
    public class SoundEventReceiver : MonoBehaviour
    {
        // TODO: Add another static reference.
        // TODO: Add a way to store reference to sound instance dynamically via inspector, and then to manipulate it.
        public enum TargetType
        {
            self,
            trigger,
            other,
            specify
        }

        // TODO: Custom settings for each.
        // TODO: Move to main enums place.
        public enum SoundAction
        {
            Play,
            Stop,
            PlayMultiple,
            StopMultiple,
            Pause,
            Resume,
            PauseMultiple,
            ResumeMultiple,
            Mute,
            Unmute,
            MuteMultiple,
            UnmuteMultiple,
            SetParameter,
            SetParameterOnMultiple
        }

        [System.Serializable]
        public struct soundEventReaction
        {
            public SoundEvent soundEvents; // TODO: Validate this!
            public SoundTriggerTag filterSoundTriggerByTag;
            public GameObject filterTriggerByReference;
            public SoundDefinition soundDefinition;
            public TargetType targetType; // TODO: Move to sound action
            public GameObject specifyOtherTarget; // TODO: Move to sound action
            public BuiltInParameter builtInParameter;
            public List<SoundAction> actions;
        }

        public List<soundEventReaction> soundEventReactions;

        // TODO: Adjust script execution order so that this doesn't break.
        private void OnEnable()
        {
            for (int i = 0; i < soundEventReactions.Count; ++i)
            {
                //Debug.Log("Event" + );
                soundEventReactions[i].soundEvents.AddReceiver(this);
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < soundEventReactions.Count; ++i)
            {
                soundEventReactions[i].soundEvents.RemoveReceiver(this);
            }
        }

        public void OnEventRaised(SoundEvent customSoundEvent, GameObject triggerObject, SoundTriggerTag soundTriggerTag, GameObject otherObject)
        {
            for (int i = 0; i < soundEventReactions.Count; ++i)
            {
                if (soundEventReactions[i].soundEvents != customSoundEvent)
                {
                    continue;
                }

                if (soundEventReactions[i].filterSoundTriggerByTag != null && soundTriggerTag != soundEventReactions[i].filterSoundTriggerByTag)
                {
                    continue; 
                }

                if (soundEventReactions[i].filterTriggerByReference != null &&
                    soundEventReactions[i].filterTriggerByReference.GetInstanceID() != triggerObject.GetInstanceID())
                {
                    continue;
                }

                // We passed all checks! Let's play the event!
                GameObject target = gameObject;
                if (soundEventReactions[i].targetType == TargetType.trigger && triggerObject != null)
                {
                    target = triggerObject;
                }
                else if (soundEventReactions[i].targetType == TargetType.other && otherObject != null)
                {
                    target = otherObject;
                }
                else if (soundEventReactions[i].targetType == TargetType.specify &&
                         soundEventReactions[i].specifyOtherTarget != null)
                {
                    target = soundEventReactions[i].specifyOtherTarget;
                }

                // TODO: Validate STOP events on sounds with a target! (or hide that field?)

                for (int j = 0; j < soundEventReactions[i].actions.Count; ++j)
                {
                    switch (soundEventReactions[i].actions[j])
                    {
                        case SoundAction.Play:
                            // TODO: Handle persistent sounds. MAYBE persistence should be on the sound definition side?
                            AudiobreadManager.Instance.PlaySound(soundEventReactions[i].soundDefinition, target);
                            break;
                        
                        case SoundAction.Stop:
                            AudiobreadManager.Instance.StopSounds(soundEventReactions[i].soundDefinition, target);
                            break;
                        
                        /*
                        case SoundAction.SetParameter:
                            // AudiobreadManager.Instance.SetAutoParameter(soundEventReactions[i].soundDefinition)
                            break;
                         
                        case SoundAction.PlayMultiple:
                            throw new NotImplementedException();
                            break;
                        
                        case SoundAction.StopMultiple:
                            throw new NotImplementedException();
                            break;
                        
                        case SoundAction.Pause:
                            throw new NotImplementedException();
                            break;
                        
                        case SoundAction.Resume:
                            throw new NotImplementedException();
                            break;
                        
                        case SoundAction.PauseMultiple:
                            throw new NotImplementedException();
                            break;
                        
                        case SoundAction.ResumeMultiple:
                            throw new NotImplementedException();
                            break;
                        
                        case SoundAction.Mute:
                            throw new NotImplementedException();
                            break;
                        
                        case SoundAction.Unmute:
                            throw new NotImplementedException();
                            break;
                        
                        case SoundAction.MuteMultiple:
                            throw new NotImplementedException();
                            break;
                        
                        case SoundAction.UnmuteMultiple:
                            throw new NotImplementedException();
                            break;
                        
                        case SoundAction.SetParameterOnMultiple:
                            throw new NotImplementedException();
                            break;
                        */
                        
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
    }
}
