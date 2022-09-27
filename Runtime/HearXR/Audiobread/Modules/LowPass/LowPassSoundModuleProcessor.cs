namespace HearXR.Audiobread
{
    public class LowPassSoundModuleProcessor : SoundModuleProcessor<LowPassSoundModule, LowPassSoundModuleDefinition>
    {
        public LowPassSoundModuleProcessor(LowPassSoundModule soundModule, ISound sound) : base(soundModule, sound) {}
        
        protected override void ApplySoundModifiers(SetValuesType setValuesType, PlaySoundFlags playSoundFlags)
        {
            throw new System.NotImplementedException();
        }
    }
}