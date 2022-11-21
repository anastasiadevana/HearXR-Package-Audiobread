namespace HearXR.Audiobread
{
    public class PitchedInstrumentSoundModuleProcessor : SoundModuleProcessor<PitchedInstrumentSoundModule, PitchedInstrumentSoundModuleDefinition>
    {
        #region Private Fields
        private bool _invalid; // TODO: Move "_invalid" up to the parent.
        private IPitchedInstrumentSound _pitchedInstrumentSound;
        #endregion
        
        public PitchedInstrumentSoundModuleProcessor(PitchedInstrumentSoundModule soundModule, ISound sound) : base(soundModule, sound) {}

        #region SoundModuleProcessor Overrides
        protected override void DoInit()
        {
            base.DoInit();
            if (MySound is IPitchedInstrumentSound)
            {
                _pitchedInstrumentSound = MySound as IPitchedInstrumentSound;
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
            
            for (var i = 0; i < properties.Length; ++i)
            {
                if (properties[i] == PitchedInstrumentSoundModuleDefinition.NoteNumberProperty)
                {
                    var noteNumber = _calculators[properties[i]].ValueContainer.IntValue;
                    // _pitchedInstrumentSound.SetNoteNumber(noteNumber);
                }
            }
        }
    }
}