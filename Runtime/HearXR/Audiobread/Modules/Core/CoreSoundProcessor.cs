namespace HearXR.Audiobread.Core
{
    public class CoreSoundProcessor : SoundModuleProcessor<CoreSoundModule, CoreSoundModuleDefinition>
    {
        public CoreSoundProcessor(CoreSoundModule soundModule, ISound sound) : base(soundModule, sound) {}

        protected override double ApplySoundModifiers(SetValuesType setValuesType, 
            PlaySoundFlags playSoundFlags = PlaySoundFlags.None, double startTime = Audiobread.INACTIVE_START_TIME)
        {
            throw new System.NotImplementedException();
        }
    }
}