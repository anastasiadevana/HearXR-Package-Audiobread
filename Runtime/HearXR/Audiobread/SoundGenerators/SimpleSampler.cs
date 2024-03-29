namespace HearXR.Audiobread
{
    // TODO: The code between SimpleSampler and AudioClip is identical. Have them inherit from the same abstract class.
    public class SimpleSampler : SoundGeneratorUnityAudio<SimpleSamplerDefinition, SimpleSampler>
    {
        #region Constructor
        public SimpleSampler(AudiobreadSource audiobreadSource) : base(audiobreadSource) {}
        #endregion

        #region Sound Abstract Methods
        protected override void DoReleaseResources()
        {
            _inUse = false;
            ((ISoundInternal) this).DeInit();
            ResetToDefaults();
        }
        #endregion

        #region SoundGeneratorUnityAudio Abstract Methods
        protected override void SetUpAudioSource()
        {
            _audioSource.clip = _soundDefinition.AudioClip;

            // Save some sample and frequency information about this clip for further calculations.
            _clipSampleRate = _soundDefinition.AudioClip.frequency;
            _clipOneSampleDuration = TimeSamplesHelper.GetSingleSampleDuration(_clipSampleRate);
            _clipTotalSamples = _soundDefinition.AudioClip.samples;
            _beforeCompletedSamplesThreshold = TimeSamplesHelper.TimeToSamples(SCHEDULING_BUFFER, _clipSampleRate);
            
            _audiobreadSource.Mode = AudiobreadSource.AudioSourceMode.ClipPlayer;
        }
        #endregion

        #region Helper Methods
        public override string ToString()
        {
            return $"- SIMPLE SAMPLER - [{Guid}] [{_soundDefinition.AudioClip.name}]";
        }
        #endregion
    }
}