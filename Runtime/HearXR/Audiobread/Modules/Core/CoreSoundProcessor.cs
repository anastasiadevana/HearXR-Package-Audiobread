namespace HearXR.Audiobread.Core
{
    public class CoreSoundProcessor : SoundModuleProcessor<CoreSoundModule, CoreSoundModuleDefinition>
    {
        public CoreSoundProcessor(CoreSoundModule soundModule, ISound sound) : base(soundModule, sound) {}

        protected override void ApplySoundModifiers(SetValuesType setValuesType, PlaySoundFlags playSoundFlags = PlaySoundFlags.None)
        {
            throw new System.NotImplementedException();
        }
    }
}