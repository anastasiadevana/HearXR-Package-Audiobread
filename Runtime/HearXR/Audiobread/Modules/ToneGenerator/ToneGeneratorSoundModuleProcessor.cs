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

            var hasMidiInfo = MySound.MidiNoteInfo != null;
            
            var properties = _soundPropertiesBySetType[setValuesType];

            // Creating separate variables for frequency and wave shape because we want to change them on a sound at the same time.
            var settings = _toneGeneratorSound.Settings;
            var frequency = settings.frequency;
            var waveShape = settings.waveShape;
            
            for (var i = 0; i < properties.Length; ++i)
            {
                if (properties[i] == ToneGeneratorSoundModuleDefinition.FrequencyProperty && !hasMidiInfo)
                {
                    // Debug.Log($"Regenerating tone frequency on {setValuesType}");
                    frequency = _calculators[properties[i]].ValueContainer.FloatValue;
                    continue;
                }

                if (properties[i] == ToneGeneratorSoundModuleDefinition.WaveShapeProperty)
                {
                    waveShape = AudiobreadManager.IntToEnum<WaveShapeEnum>(_calculators[properties[i]].ValueContainer.IntValue);
                    continue;
                }
            }

            // If we have MIDI note info, override the frequency completely.
            if (hasMidiInfo)
            {
                // Convert MIDI note number to frequency.
                frequency = AudiobreadManager.NoteNumberToFrequency(MySound.MidiNoteInfo.noteNumber);
            }
            
            settings.frequency = frequency;
            settings.waveShape = waveShape;

            _toneGeneratorSound.Settings = settings;
        }
    }
}