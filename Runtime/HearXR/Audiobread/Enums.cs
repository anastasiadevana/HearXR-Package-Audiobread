using System;

namespace HearXR.Audiobread
{
    [Flags] public enum InitSoundFlags
    {
        None = 0,
        PersistInstance = 1
    }
    
    [Flags] public enum PlaySoundFlags
    {                    
        None                        = 0,
        Instant                     = 1 << 0, // TODO: Ignore fades
        DoNotResumeIfPaused         = 1 << 1, // TODO: Do not resume if paused
        DoNotRetriggerIfPlaying     = 1 << 2, // TODO: Do not retrigger if playing
        KeepLastRandomizationValues = 1 << 3, // TODO: Do not re-randomize values or clips
        PersistInstance             = 1 << 4, // TODO: Replace "persist" stuff with this.
        PlayNext                    = 1 << 5  // TODO: Play next sound if continuous
    }
    
    [Flags] public enum StopSoundFlags
    {
        None                   = 0,
        Instant                = 1 << 0, // Ignores fades
        UnsetPersistentFlag    = 1 << 1
    }
    
    [Flags]
    public enum SoundStatus
    {
        None          = 0,
        Invalid       = 1 << 0,
        Initialized   = 1 << 1,
        ReadyToPlay   = 1 << 2,
        Paused        = 1 << 3,
        FadingIn      = 1 << 4,
        FadingOut     = 1 << 5,
        Persistent    = 1 << 6,
        Muted         = 1 << 7,
        Virtual       = 1 << 8
    }

    public enum PlaybackState
    {
        Stopped,
        PlayInitiated,
        Playing,
        StopInitiated
    }
    
    public enum SoundType
    {
        OneShot,
        Continuous
    }

    public enum SchedulableSoundState
    {
        None,
        Scheduled,
        Playing,
        Completing
    }

    public enum MusicalSoundState
    {
        None,
        Intro,
        Middle,
        Outro
    }
    
    public enum SpatializationType
    {
        TwoDee,
        ThreeDee,
        Spatialized
    }

    public enum SetValuesType
    {
        OnPreparedToPlay,
        OnBeforePlay,
        OnBeforeChildPlay,
        OnUpdate,
        All
    }
    
    public enum ParentSoundType
    {
        OneShot,
        Continuous
    }

    public enum ParentSoundPlaybackOrder
    {
        Random,
        Sequential/*, // TODO: Concurrent should be turned into a blend or something.
        Concurrent*/
    }
        
    public enum ParentSoundRepeatType
    {
        Child,
        Self
    }
    
    public enum WaveShapeEnum
    {
        Sin,
        Square,
        Sawtooth,
        Triangle
    }
}