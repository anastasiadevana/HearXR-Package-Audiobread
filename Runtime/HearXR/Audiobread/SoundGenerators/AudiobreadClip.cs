namespace HearXR.Audiobread
{
    // Actually plays the sound.
    public class AudiobreadClip : SoundGeneratorUnityAudio<AudiobreadClipDefinition, AudiobreadClip>
    {
        #region Constructor
        public AudiobreadClip(AudiobreadSource audiobreadSource) : base(audiobreadSource) {}
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
            return $"- AudiobreadClip - [{Guid}] [{_soundDefinition.AudioClip.name}]";
        }
        #endregion
    }
}
