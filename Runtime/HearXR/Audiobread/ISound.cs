using System.Collections.Generic;
using HearXR.Audiobread.SoundProperties;
using UnityEngine;

namespace HearXR.Audiobread
{
    // TODO: We need a PlayDelayed of some kind.
    
    /// <summary>
    /// Shared between any sound node.
    /// </summary>
    public interface ISound
    {
        #region Events
        event TimedSoundAction OnBegan;
        event SoundAction OnEnded;
        event TimedSoundAction OnChildDidBegin;
        event TimedSoundAction OnBeforeEnded;
        event TimedSoundAction OnScheduled;
        #endregion
        
        #region Properties
        ISound ParentSound { get; set; }

        // bool CanBeRegistered { get; } // TODO: Delete if not used.

        bool HasChildren { get; }

        bool HasParent { get; }
        
        ISoundDefinition SoundDefinition { get; }

        System.Guid Guid { get; }
        
        SoundStatus Status { get; }
        
        PlaybackState PlaybackState { get; }
        
        GameObject SoundSourceObject { get; set; }
        
        MidiNoteInfo MidiNoteInfo { get; set; }
        
        SchedulableSoundState SchedulableState { get; }
        #endregion

        #region Methods
        ISound GetTopParent();
        
        bool TryGetRegisteredParent(out ISound parentSound);
        
        void Play(PlaySoundFlags playSoundFlags = PlaySoundFlags.None);

        void SetParameter(Parameter parameter, float parameterValue);
        
        void PlayScheduled(double startTime, PlaySoundFlags playSoundFlags = PlaySoundFlags.None);

        void Stop(StopSoundFlags stopSoundFlags = StopSoundFlags.None);
        
        void Pause();

        void Resume();

        void Mute();

        void Unmute();
        
        bool IsValid();
        
        bool IsInitialized();

        bool IsReadyToPlay();

        bool IsPlaying();

        bool IsStopped();

        bool IsPlayingOrTransitioning();

        bool IsPaused();

        bool IsStoppedOrPaused();

        bool IsMuted();

        bool IsVirtual();
        
        bool IsPersistent();

        void SetPersistent(bool persistent);
        
        bool IsContinuous();
        
        void UpdateSound();
        
        bool GetCalculators(out Dictionary<SoundProperty, Calculator> calculators);

        void ReleaseResources(); // TODO: Move to internal.

        void RefreshSoundDefinition(); // TODO: Move to internal.

        TProperties GetModuleSoundDefinitions<TProperties>(SoundModule soundModule) where TProperties : SoundModuleDefinition;

        void RegisterTimeBeforeListener(ITimeBeforeListener listener, double timeBefore);

        void UnregisterTimeBeforeListener(ITimeBeforeListener listener);
        #endregion
    }

    public interface ISound<in TDefinition> : ISound where TDefinition : ISoundDefinition {}
}