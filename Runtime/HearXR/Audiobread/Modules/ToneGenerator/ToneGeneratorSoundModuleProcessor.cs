using HearXR.Audiobread.SoundProperties;
using UnityEngine;

namespace HearXR.Audiobread
{
    public class ToneGeneratorSoundModuleProcessor : SoundModuleProcessor<ToneGeneratorSoundModule, ToneGeneratorSoundModuleDefinition>
    {
        #region Private Fields
        private bool _invalid; // TODO: Move "_invalid" up to the parent.
        private IToneGeneratorSound _toneGeneratorSound;
        #endregion
        
        public ToneGeneratorSoundModuleProcessor(ToneGeneratorSoundModule soundModule, ISound sound) : base(soundModule, sound) {}

        #region SoundModuleProcessor Overrides
        protected override void DoInit()
        {
            base.DoInit();
            if (MySound is IToneGeneratorSound)
            {
                _toneGeneratorSound = MySound as IToneGeneratorSound;
            }
            else
            {
                _invalid = true;
            }
        }
        #endregion
        
        protected override void ApplySoundModifiers(ref Sound.SoundInstancePlaybackInfo instancePlaybackInfo, SetValuesType setValuesType, 
            PlaySoundFlags playSoundFlags = PlaySoundFlags.None)
        {
            if (!MySound.IsValid() || _invalid) return;
            
            var properties = _soundPropertiesBySetType[setValuesType];

            // Creating separate variables for frequency and wave shape because we want to change the on a sound at the same time.
            var settings = _toneGeneratorSound.Settings;
            var frequency = settings.frequency;
            var waveShape = settings.waveShape;
            
            for (var i = 0; i < properties.Length; ++i)
            {
                if (properties[i] == ToneGeneratorSoundModuleDefinition.FrequencyProperty)
                {
                    // Debug.Log($"Regenerating tone frequency on {setValuesType}");
                    frequency = _calculators[properties[i]].ValueContainer.FloatValue;
                    continue;
                }

                if (properties[i] == ToneGeneratorSoundModuleDefinition.WaveShapeProperty)
                {
                    waveShape = Audiobread.IntToEnum<WaveShapeEnum>(_calculators[properties[i]].ValueContainer.IntValue);
                    continue;
                }
            }

            settings.frequency = frequency;
            settings.waveShape = waveShape;
            
            // If we have MIDI note info, override the frequency completely.
            if (MySound.MidiNoteInfo != null)
            {
                // Debug.Log("Sound has midi note info");
                
                // Convert MIDI note number to frequency.
                settings.frequency = Audiobread.NoteNumberToFrequency(MySound.MidiNoteInfo.noteNumber);
            }
            
            _toneGeneratorSound.Settings = settings;
        }
    }
}