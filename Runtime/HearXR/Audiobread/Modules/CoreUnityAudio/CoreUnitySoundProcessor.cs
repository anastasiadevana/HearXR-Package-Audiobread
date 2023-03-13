using HearXR.Audiobread.SoundProperties;
using UnityEngine;

namespace HearXR.Audiobread.Core
{
    public class CoreUnitySoundProcessor : SoundModuleProcessor<CoreUnitySoundModule, CoreUnitySoundModuleDefinition>, 
                                            IStopControllerProcessor, ITimeBeforeListener
    {
        #region Private Fields
        private bool _initSoundSource;
        private AudiobreadSource _audiobreadSource;
        private AudioSource _audioSource;
        private IPitchedInstrumentSound _pitchedInstrumentSound;
        private bool _isPitchedInstrument;
        private VolumeCalculator _volumeCalculator;
        private bool _hasFadeIn;
        private bool _hasFadeOut;
        private StopControllerStopCallback _stopCallbackWaiting;
        private ValueContainer _midiNoteVelocityInfluence = new FloatValueContainer(1.0f);
        #endregion

        #region Constructor
        public CoreUnitySoundProcessor(CoreUnitySoundModule soundModule, ISound sound) : base(soundModule, sound) {}
        #endregion

        #region SoundModuleProcessor Overrides
        protected override void DoInit()
        {
            base.DoInit();
            if (MySound is IPitchedInstrumentSound)
            {
                _pitchedInstrumentSound = MySound as IPitchedInstrumentSound;
                _isPitchedInstrument = true;
            }
        }
        
        protected override void DoApplySoundDefinitionToUnityAudio(AudiobreadSource audiobreadSource)
        {
            base.DoApplySoundDefinitionToUnityAudio(audiobreadSource);
            FindAudioSource(audiobreadSource);
        }

        protected override void PostInitCalculators()
        {
            base.PostInitCalculators();
            
            _hasFadeIn = ModuleSoundDefinition.fadeInDefinition.duration > Mathf.Epsilon;
            _hasFadeOut = ModuleSoundDefinition.fadeOutDefinition.duration > Mathf.Epsilon;

            // Add fades to the volume calculator.
            _volumeCalculator = (VolumeCalculator) _calculators[BuiltInData.Properties.GetSoundPropertyByType<Volume>()];
            _volumeCalculator.SetFades(ModuleSoundDefinition.fadeInDefinition, ModuleSoundDefinition.fadeOutDefinition);
            
            // TODO: Do we need to unsubscribe at some point?
            _volumeCalculator.OnFadeOutFinished += HandleFadeOutFinished;
            _volumeCalculator.OnFadeOutFinished += HandleFadeInFinished;
            
            if (_volumeCalculator.HasFadeOut)
            {
                // TODO: Do we need to unregister at some point?
                MySound.RegisterTimeBeforeListener(this, ModuleSoundDefinition.fadeOutDefinition.duration);   
            }
        }
        
        protected override void OnSoundBegan(ISound sound, double startTime)
        {
            base.OnSoundBegan(sound, startTime);
            
            // TODO: Compare sound with _sound.
            _volumeCalculator.OnSoundBegan();
        }
        
        protected override void OnUnityAudioGeneratorTick(ref Sound.SoundInstancePlaybackInfo instancePlaybackInfo)
        {
            ApplySoundModifiers(ref instancePlaybackInfo, SetValuesType.OnUpdate);
        }
        
        protected override void ApplySoundModifiers(ref Sound.SoundInstancePlaybackInfo instancePlaybackInfo, SetValuesType setValuesType, 
            PlaySoundFlags playSoundFlags = PlaySoundFlags.None)
        {
            if (!MySound.IsValid() || !_initSoundSource) return;

            var hasMidiInfo = MySound.MidiNoteInfo != null;
            
            // Add MIDI velocity influence.
            if (setValuesType == SetValuesType.OnBeforePlay && hasMidiInfo)
            {
                _midiNoteVelocityInfluence.FloatValue = MySound.MidiNoteInfo.velocity;
                _volumeCalculator.AddInfluence(_midiNoteVelocityInfluence);
            }

            var properties = _soundPropertiesBySetType[setValuesType];

            for (var i = 0; i < properties.Length; ++i)
            {
                if (!_calculators[properties[i]].Active) continue;
                
                // Debug.Log($"{setValuesType}--{properties.Length} {properties[i].ShortName}");

                if (properties[i] == CoreUnitySoundModuleDefinition.NoteNumberProperty)
                {
                    if (!_isPitchedInstrument)
                    {
                        Debug.LogWarning("Cannot apply note number property to this type of sound.");
                    }
                    else
                    {
                        if (hasMidiInfo)
                        {
                            if (setValuesType == SetValuesType.OnBeforePlay)
                            {
                                Debug.LogWarning("This sound is triggered with a midi note number. Ignoring the note number setting.");   
                            }
                        }
                        else
                        {
                            SetNoteNumber(_calculators[CoreUnitySoundModuleDefinition.NoteNumberProperty].ValueContainer.IntValue);   
                        }
                    }
                    continue;
                }
                
                // Pitch
                if (properties[i] == CoreUnitySoundModuleDefinition.PitchProperty)
                {
                    if (_isPitchedInstrument)
                    {
                        if (setValuesType == SetValuesType.OnBeforePlay)
                        {
                            Debug.LogWarning("This sound is a pitched instrument. Use the note number sound property to adjust pitch.");   
                        }
                    }
                    else
                    {
                        var value = _calculators[properties[i]].ValueContainer.FloatValue;
                        _audioSource.pitch = value;
                    }
                    continue;
                }
                
                // Volume
                if (properties[i] == CoreUnitySoundModuleDefinition.VolumeProperty)
                {
                    var value = _calculators[properties[i]].ValueContainer.FloatValue;
                    _audioSource.volume = value;
                    continue;
                }

                // Delay
                if (properties[i] == CoreUnitySoundModuleDefinition.DelayProperty)
                {
                    if (setValuesType == SetValuesType.OnBeforePlay)
                    {
                        var delay = _calculators[properties[i]].ValueContainer.FloatValue;
                        if (delay > 0)
                        {
                            if (instancePlaybackInfo.startTime < AudioSettings.dspTime)
                            {
                                instancePlaybackInfo.startTime = AudioSettings.dspTime;
                            }
                            instancePlaybackInfo.startTime += delay;
                            instancePlaybackInfo.scheduledStart = true;
                        }
                    }
                    continue;
                }
                
                // Time duration
                if (properties[i] == CoreUnitySoundModuleDefinition.TimeDurationProperty)
                {
                    if (setValuesType == SetValuesType.OnBeforePlay)
                    {
                        // TODO: Shouldn't this be handled here?
                        // Ignore the duration property if it's played as a MIDI note.
                        if (MySound.MidiNoteInfo != null &&
                            MySound.MidiNoteInfo.duration > AudiobreadManager.INVALID_TIME_DURATION)
                        {
                            Debug.LogWarning($"This sound is triggered with a midi note duration ({MySound.MidiNoteInfo.duration}). Ignoring the duration setting.");
                            continue;
                        }
                        
                        // TODO: Validate the value?
                        var duration = _calculators[properties[i]].ValueContainer.DoubleValue;
                        // Debug.Log($"Setting duration to {duration}");
                        instancePlaybackInfo.duration = duration;
                        instancePlaybackInfo.scheduledEnd = true;
                        instancePlaybackInfo.scheduledStart = true;
                    }

                    continue;
                }
                
                // TODO: Add offset.
                // else if (properties[i] == _offsetProperty)
                // {
                //     var audioClip = _soundDefinition.AudioClip;
                //     _audioSource.timeSamples = TimeSamplesHelper.ValidateAudioClipOffset(in audioClip, value);
                // }
            }

            // Deal with volume fades.
            if (setValuesType == SetValuesType.OnPreparedToPlay)
            {
                _volumeCalculator.PrepareToPlay();
            }
            
            // If we have MIDI note info, override the frequency completely.
            if (setValuesType == SetValuesType.OnBeforePlay && _isPitchedInstrument && hasMidiInfo)
            {
                SetNoteNumber(MySound.MidiNoteInfo.noteNumber);
            }
        }
        #endregion

        #region ITimeBeforeListener Methods
        public void HandleTimeRemainingEvent(double timeRemaining)
        {
            _volumeCalculator.StartFadeOut();
        }
        #endregion

        #region Event Handlers
        private void HandleFadeOutFinished()
        {
            _stopCallbackWaiting?.Invoke();
            _stopCallbackWaiting = null;
        }

        private void HandleFadeInFinished() {}
        #endregion

        #region Private Methods
        private void FindAudioSource(AudiobreadSource audiobreadSource)
        {
            if (_initSoundSource) return;
            _audiobreadSource = audiobreadSource;
            _audioSource = _audiobreadSource.gameObject.GetComponent<AudioSource>();
            _initSoundSource = true;
        }

        /// <summary>
        /// Apply MIDI note number.
        /// </summary>
        /// <param name="noteNumber"></param>
        private void SetNoteNumber(int noteNumber)
        {
            var baseNoteNumber = _pitchedInstrumentSound.BaseNoteNumber(noteNumber);
            var value = AudiobreadManager.NoteNumberToFrequency(noteNumber, baseNoteNumber);
                    
            _audioSource.pitch = value; 
        }
        #endregion

        #region IStopControllerProcessor Methods
        public bool CanStopNow(StopSoundFlags stopSoundFlags)
        {
            return Sound.HasStopFlag(stopSoundFlags, StopSoundFlags.Instant) || !_hasFadeOut;
        }
        
        public void RequestStop(StopSoundFlags stopSoundFlags, StopControllerStopCallback stopCallback)
        {
            if (CanStopNow(stopSoundFlags))
            {
                stopCallback();
                return;
            }
            
            if (!_volumeCalculator.OnStopSound(stopSoundFlags))
            {
                stopCallback();
                return;
            }

            _stopCallbackWaiting = stopCallback;
        }
        #endregion
    }
}