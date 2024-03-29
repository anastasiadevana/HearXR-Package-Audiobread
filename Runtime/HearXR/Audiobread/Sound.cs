using System;
using System.Collections.Generic;
using System.Linq;
using HearXR.Audiobread.SoundProperties;
using UnityEngine;

namespace HearXR.Audiobread
{
    public abstract class Sound : ISoundInternal
    {
        #region Data Structures
        public class SoundInstancePlaybackInfo
        {
            public bool scheduledStart = false;
            public double startTime;
            public bool scheduledEnd = false;
            public double duration;
        }
        #endregion
        
        #region ISound Events
        public event TimedSoundAction OnBegan;
        private readonly object _onBeganLock = new object();
        event TimedSoundAction ISound.OnBegan
        {
            add { lock (_onBeganLock) { OnBegan += value; } }
            remove { lock (_onBeganLock) { OnBegan -= value; } }
        }

        public event SoundAction OnEnded;
        private readonly object _onEndedLock = new object();
        event SoundAction ISound.OnEnded
        {
            add { lock (_onEndedLock) { OnEnded += value; } }
            remove { lock (_onEndedLock) { OnEnded -= value; } }
        }
        
        public event TimedSoundAction OnChildDidBegin;
        private readonly object _onChildDidBeginLock = new object();
        event TimedSoundAction ISound.OnChildDidBegin
        {
            add { lock (_onChildDidBeginLock) { OnChildDidBegin += value; } }
            remove { lock (_onChildDidBeginLock) { OnChildDidBegin -= value; } }
        }
        
        public event TimedSoundAction OnScheduled;
        private readonly object _onScheduledLock = new object();

        event TimedSoundAction ISound.OnScheduled
        {
            add { lock (_onScheduledLock) { OnScheduled += value; } }
            remove { lock (_onScheduledLock) { OnScheduled -= value; } }
        }

        public event TimedSoundAction OnBeforeEnded;
        private readonly object _onBeforeEndedLock = new object();

        event TimedSoundAction ISound.OnBeforeEnded
        {
            add { lock (_onBeforeEndedLock) { OnBeforeEnded += value; } }
            remove { lock (_onBeforeEndedLock) { OnBeforeEnded -= value; } }
        }
        
        protected event Action InitSoundModuleProcessor;
        protected event Action SoundDefinitionChangedEvent;
        public delegate void TickAction(ref SoundInstancePlaybackInfo instancePlaybackInfo);
        protected event TickAction SoundUpdateTickEvent;
        protected event TickAction UnityAudioGeneratorTickEvent;
        public delegate void BeforePlayAction(ref SoundInstancePlaybackInfo instancePlaybackInfo, PlaySoundFlags playSoundFlags = PlaySoundFlags.None);
        public event BeforePlayAction OnPreparedToPlay;
        public event BeforePlayAction BeforeChildPlayEvent;
        public event BeforePlayAction BeforePlayEvent;
        protected event Action<ISound> ParentSetEvent;
        #endregion

        #region Constants
        // TODO: These need to be moved elsewhere and documented.
        protected const double SCHEDULING_BUFFER = 0.1f; // TODO: Move to Audiobread core.
        protected const int SAFETY_SAMPLE_BUFFER = 5;
        #endregion

        #region Constructor
        protected Sound()
        {
            _guid = Guid.NewGuid();

            // TODO: We can do better than this, I recon.
#if UNITY_EDITOR
            _soundRegistry = !Application.isPlaying ? SoundRegistry.Instance : AudiobreadManager.Instance.Registry;
#else
            _soundRegistry = AudiobreadManager.Instance.Registry;
#endif
        }
        #endregion

        #region ISound Properties
        public Guid Guid => _guid;
        private Guid _guid;

        public SoundStatus Status => _status;
        private SoundStatus _status;

        public PlaybackState PlaybackState => _playbackState;
        private PlaybackState _playbackState = PlaybackState.Stopped;

        public GameObject SoundSourceObject
        {
            get => _soundSourceObject;
            set
            {
                _soundSourceObject = value;
                PostSetSoundSourceObject();
            }
        }
        protected GameObject _soundSourceObject;

        public MidiNoteInfo MidiNoteInfo
        {
            get => _midiNoteInfo;
            set
            {
                _midiNoteInfo = value;
                PostSetMidiNoteInfo();
            }
        }

        protected MidiNoteInfo _midiNoteInfo;

        public ISound ParentSound
        {
            get => _parentSound;
            set
            {
                if (_parentSound == value) return;
                var previousParentSound = _parentSound;
                _parentSound = value;
                _haveParent = (value != null);
                PostSetParent(previousParentSound, _parentSound);
            }
        }
        protected ISound _parentSound;

        public bool HasChildren => _hasChildren;
        protected bool _hasChildren;

        // TODO: Strangely, this falsely returns true when the pool starts swapping.
        public bool HasParent => _haveParent;
        private bool _haveParent;

        SchedulableSoundState ISound.SchedulableState => _schedulableState;
        protected SchedulableSoundState _schedulableState;

        protected SoundInstancePlaybackInfo _instancePlaybackInfo;
        
        SoundModuleGroupProcessor ISoundInternal.SoundModuleGroupProcessor => _soundModuleGroupProcessor;
        #endregion

        #region Properties
        protected bool Registered => _registered;
        private bool _registered;
        #endregion
        
        #region Protected Fields
        protected SoundModuleGroupProcessor _soundModuleGroupProcessor;
        protected bool _hasSoundModuleGroupProcessor;
        #endregion
        
        #region Private Fields
        private SoundType _soundType;
        private readonly SoundRegistry _soundRegistry;
        private bool _haveChildren;
        private ISound _registeredParent;
        #endregion

        #region ISound Properties to Abstract
        public abstract ISoundDefinition SoundDefinition { get; }
        // TODO: Move these abstract methods into a correct region.
        public abstract void Play(PlaySoundFlags playSoundFlags = PlaySoundFlags.None);
        public abstract void ReleaseResources();
        public abstract bool Pitched { get; }
        public abstract int BaseNoteNumber { get; }
        public abstract Dictionary<Parameter, float> ParameterValues { get; }
        #endregion

        #region ISchedulable Methods to Abstract
        public abstract void PlayScheduled(double startTime, PlaySoundFlags playSoundFlags = PlaySoundFlags.None);
        #endregion

        #region ISoundInternal Methods
        void ISoundInternal.PrepareToPlay()
        {
            if (!CanPrepareToPlay()) return;
            
            // Avoid firing the event twice.
            if (!IsReadyToPlay())
            {
                SetStatus(SoundStatus.ReadyToPlay, true);
                OnPreparedToPlay?.Invoke(ref _instancePlaybackInfo);   
            }
        }

        void ISoundInternal.DeInit()
        {
            ResetStatus(SoundStatus.None);
            OnDeInit();
        }
        
        public abstract void AddTrackedParameters(List<Parameter> parameters);
        #endregion

        #region Abstract Methods
        protected abstract void DoStop(StopSoundFlags stopFlags);
        protected abstract void DoPause();
        protected abstract void DoResume();
        protected abstract void DoMute();
        protected abstract void DoUnmute();
        internal abstract void AddSoundPropertyInfluence<T>(ValueContainer influence) where T : SoundProperty;
        internal abstract void AddSoundPropertyInfluence(SoundProperty property, ValueContainer influence);
        internal abstract void RemoveSoundPropertyInfluence<T>(ValueContainer influence) where T : SoundProperty;

        protected abstract void DoRefreshSoundDefinition();
        #endregion

        #region Overridable Methods
        internal virtual void DoUpdate() {}

        protected virtual void PostSetSoundSourceObject() {}

        protected virtual void PostSetMidiNoteInfo() {}

        protected virtual void PostSetParent(ISound previousParent, ISound newParent)
        {
            // TODO: Not sure how this would happen or what to do about this.
            if (newParent == null || newParent == previousParent) return;
            ((ISoundInternal) this).InvokeOnSetParent(newParent);
        }

        protected virtual void PostSetStatus(SoundStatus statusFlag, bool wasSet, bool isSet) {}

        protected virtual void PostResetStatus(SoundStatus previousStatus, SoundStatus newStatus) {}

        protected virtual void PostPlaybackStateChanged(PlaybackState previousState, PlaybackState newState) {}

        protected virtual bool CanPrepareToPlay()
        {
            return IsValid() && IsInitialized();
        }
        
        protected virtual bool CanStop(StopSoundFlags stopSoundFlags = StopSoundFlags.None)
        {
            return !IsStopped();
        }

        protected virtual bool CanPause()
        {
            return IsValid() && !IsStopped();
        }

        protected virtual bool CanResume()
        {
            return IsValid() && IsPaused();
        }

        protected virtual bool CanMute()
        {
            return !IsMuted();
        }

        protected virtual bool CanUnmute()
        {
            return IsMuted();
        }

        protected virtual bool ShouldUpdate()
        {
            return IsValid();
        }
        
        protected virtual void OnDeInit() {}

        protected virtual void SubscribeSoundModuleProcessorsToEvents()
        {
            InitSoundModuleProcessor += _soundModuleGroupProcessor.Init;
            SoundDefinitionChangedEvent += _soundModuleGroupProcessor.HandleSoundDefinitionChanged;
            SoundUpdateTickEvent += _soundModuleGroupProcessor.HandleSoundUpdateTick;
            UnityAudioGeneratorTickEvent += _soundModuleGroupProcessor.HandleUnityAudioGeneratorTickEvent;
            OnPreparedToPlay += _soundModuleGroupProcessor.HandlePreparedToPlay;
            BeforeChildPlayEvent += _soundModuleGroupProcessor.HandleBeforeChildPlay;
            BeforePlayEvent += _soundModuleGroupProcessor.HandleBeforePlay;
            ParentSetEvent += _soundModuleGroupProcessor.HandleSetParent;
            OnBegan += _soundModuleGroupProcessor.HandleOnSoundBegan;
        }
        
        protected virtual void UnsubscribeSoundModuleProcessorFromEvents()
        {
            // TODO: When should we call this?
            InitSoundModuleProcessor -= _soundModuleGroupProcessor.Init;
            SoundDefinitionChangedEvent -= _soundModuleGroupProcessor.HandleSoundDefinitionChanged;
            SoundUpdateTickEvent -= _soundModuleGroupProcessor.HandleSoundUpdateTick;
            UnityAudioGeneratorTickEvent -= _soundModuleGroupProcessor.HandleUnityAudioGeneratorTickEvent;
            OnPreparedToPlay -= _soundModuleGroupProcessor.HandlePreparedToPlay;
            BeforeChildPlayEvent -= _soundModuleGroupProcessor.HandleBeforeChildPlay;
            BeforePlayEvent -= _soundModuleGroupProcessor.HandleBeforePlay;
            ParentSetEvent -= _soundModuleGroupProcessor.HandleSetParent;
            OnBegan -= _soundModuleGroupProcessor.HandleOnSoundBegan;
        }
        #endregion
        
        #region ISound Methods
        // TODO: Move stuff to appropriate regions.
        public abstract void SetParameter(Parameter parameter, float parameterValue);

        public void UpdateSound()
        {
            // TODO: AudiobreadClip SHOULD be able to be played without being a proper first-class sound citizen.
            Update();
        }
        
        public bool IsPersistent()
        {
            return HasStatus(SoundStatus.Persistent);
        }

        public bool IsContinuous()
        {
            return HasType(SoundType.Continuous);
        }

        public bool GetCalculators(out Dictionary<SoundProperty, Calculator> calculators)
        {
            // TODO: SoundModule hookup.
            // calculators = _calculators;
            calculators = null;
            return true;
        }

        public ISound GetTopParent()
        {
            if (ParentSound == null) return this;
            return ParentSound.GetTopParent();
        }
        
        public virtual bool TryGetRegisteredParent(out ISound parentSound)
        {
            if (HasParent)
            {
                return _parentSound.TryGetRegisteredParent(out parentSound);
            }
            
            if (_registered)
            {
                parentSound = this;
                return true;
            }
            
            Debug.LogError($"{GetType().Name} trying to get REGISTERED_PARENT - I don't have a parent, but I'm not registered either?!");
            parentSound = default;
            return false;
        }
        
        public bool IsValid()
        {
            return HasStatus(SoundStatus.Invalid) == false;
        }
        
        public bool IsInitialized()
        {
            return HasStatus(SoundStatus.Initialized);
        }
        
        public bool IsReadyToPlay()
        {
            return HasStatus(SoundStatus.ReadyToPlay);
        }

        public bool IsPlaying()
        {
            return (_playbackState == PlaybackState.Playing);
        }

        public bool IsStopped()
        {
            return (_playbackState == PlaybackState.Stopped);
        }
        
        public bool IsPlayingOrTransitioning()
        {
            return (_playbackState == PlaybackState.PlayInitiated ||
                    _playbackState == PlaybackState.Playing ||
                    _playbackState == PlaybackState.StopInitiated);
        }

        public bool IsPlayingOrStopping()
        {
            return (_playbackState == PlaybackState.Playing ||
                    _playbackState == PlaybackState.StopInitiated);
        }

        public bool IsPaused()
        {
            return (_status & SoundStatus.Paused) == SoundStatus.Paused;
        }

        public bool IsStoppedOrPaused()
        {
            return IsStopped() || IsPaused();
        }

        public bool IsMuted()
        {
            return HasStatus(SoundStatus.Muted);
        }

        public bool IsVirtual()
        {
            return HasStatus(SoundStatus.Virtual);
        }

        public void Stop(StopSoundFlags stopSoundFlags = StopSoundFlags.None)
        {
            // Debug.Log($"Sound.Stop {this}");
            
            // TODO: Take into account sound flags.
            if (!CanStop(stopSoundFlags))
            {
                return;
            }
            SetStatus(SoundStatus.Paused, false);
            SetPlaybackState(PlaybackState.StopInitiated);
            DoStop(stopSoundFlags);
        }

        public void Pause()
        {
            if (!CanPause()) return;
            SetStatus(SoundStatus.Paused, true);
            
            // TODO: Pause any fades.
            // TODO: If the fades actually get calculated inside the Calculate() function AND the SoundUnit updates all
            //       its own calculators when not stopped or paused, it might just work!
            DoPause();
        }

        public void Resume()
        {
            if (!CanResume()) return;
            SetStatus(SoundStatus.Paused, false);
            
            // TODO: Resume any fades.
            DoResume();
        }

        public void Mute()
        {
            if (!CanMute()) return;
            DoMute();
            SetStatus(SoundStatus.Muted, true);
        }

        public void Unmute()
        {
            if (!CanUnmute()) return;
            DoUnmute();
            SetStatus(SoundStatus.Muted, false);
        }
        
        public void RefreshSoundDefinition()
        {
            if (HasStatus(SoundStatus.Initialized))
            {
                DoRefreshSoundDefinition();
            }
        }

        public abstract TProperties GetModuleSoundDefinitions<TProperties>(SoundModule soundModule) where TProperties : SoundModuleDefinition;

        protected List<TimeBeforeListenerSettings> _timeBeforeListeners = new List<TimeBeforeListenerSettings>();

        protected class TimeBeforeListenerSettings
        {
            public ITimeBeforeListener listener;
            public double timeBefore;
            public bool triggered;
        }
        
        public void RegisterTimeBeforeListener(ITimeBeforeListener listener, double timeBefore)
        {
            // TODO: Check if one already exists.
            var listenerSettings = new TimeBeforeListenerSettings {listener = listener, timeBefore = timeBefore};
            _timeBeforeListeners.Add(listenerSettings);
            
            // TODO: Sort by timeBefore in descending order.
        }

        public void UnregisterTimeBeforeListener(ITimeBeforeListener listener)
        {
            // TODO: Unregister.
        }
        #endregion

        #region Public Methods
        public void SetPersistent(bool persistent)
        {
            bool wasPersistent = IsPersistent();
            if (wasPersistent == persistent) return;
            
            SetStatus(SoundStatus.Persistent, persistent);
            
            // This used to be a persistent sound, and now it's not. 
            // If we're stopped, go ahead and release the resources and disappear forever.
            if (!persistent && !IsPlayingOrTransitioning() && !HasParent)
            {
                ReleaseAndUnregisterSelf();
                return;
            }

            RegisterSelf();
        }
        #endregion
        
        #region Protected Methods
        protected void SetStatus(SoundStatus status, bool set)
        {
            var wasSet = HasStatus(status);
            if (wasSet == set) return;
            _status ^= status;
            PostSetStatus(status, wasSet, set);
        }
        
        protected void ResetStatus(SoundStatus newStatus)
        {
            SoundStatus previousStatus = _status;
            _status = newStatus;
            PostResetStatus(previousStatus, newStatus);
        }

        private bool HasStatus(SoundStatus status)
        {
            return (_status & status) == status;
        }

        protected void SetType(SoundType type, bool set)
        {
            bool wasSet = HasType(type);
            if (wasSet == set) return;
            _soundType ^= type;
        }
        
        protected bool HasType(SoundType soundType)
        {
            return (_soundType & soundType) == soundType;
        }

        protected void SetPlaybackState(PlaybackState newState)
        {
            var previousState = _playbackState;
            _playbackState = newState;
            if (newState != previousState)
            {
                PostPlaybackStateChanged(previousState, newState);    
            }
        }

        protected void InvokeInitSoundModuleProcessor()
        {
            InitSoundModuleProcessor?.Invoke();
        }
        
        protected void InvokeOnBegan(ISound sound, double time)
        {
            // Debug.Log($"{this} began");
            OnBegan?.Invoke(sound, time);
        }

        protected void InvokeOnEnded(ISound sound)
        {
            OnEnded?.Invoke(sound);
        }
        
        protected void InvokeOnChildBegan(ISound child, double time)
        {
            OnChildDidBegin?.Invoke(child, time);
        }

        protected void InvokeOnScheduled(ISound sound, double time)
        {
            OnScheduled?.Invoke(sound, time);
        }

        protected void InvokeOnBeforeEnded(ISound sound, double time)
        {
            OnBeforeEnded?.Invoke(sound, time);
        }
        
        protected void InvokeSoundDefinitionChanged()
        {
            SoundDefinitionChangedEvent?.Invoke();
        }
        protected void InvokeSoundUpdateTickEvent()
        {
            SoundUpdateTickEvent?.Invoke(ref _instancePlaybackInfo);
        }

        protected void InvokeUnityAudioGeneratorTickEvent()
        {
            UnityAudioGeneratorTickEvent?.Invoke(ref _instancePlaybackInfo);
        }
        protected void InvokeOnBeforeChildPlay(PlaySoundFlags playSoundFlags)
        {
            BeforeChildPlayEvent?.Invoke(ref _instancePlaybackInfo, playSoundFlags);
        }
        
        protected void InvokeOnBeforePlay(PlaySoundFlags playSoundFlags)
        {
            BeforePlayEvent?.Invoke(ref _instancePlaybackInfo, playSoundFlags);
        }
        
        void ISoundInternal.InvokeOnSetParent(ISound parentSound)
        {
            ParentSetEvent?.Invoke(parentSound);
        }

        protected void RegisterSelf()
        {
            if (_registered || HasParent) return;
            _soundRegistry.RegisterSoundInstance(this);
            _registered = true;
        }
        
        protected void ReleaseAndUnregisterSelf()
        {
            ReleaseResources();
            if (_registered)
            {
                _soundRegistry.UnregisterSoundInstance(this);
                _registered = false;   
            }
        }
        #endregion

        #region Internal Methods
        internal void Update()
        {
            if (!ShouldUpdate()) return;
            DoUpdate();
        }
        #endregion
        
        #region Static Methods
        protected static bool PersistRequested(InitSoundFlags initSoundFlags)
        {
            return (initSoundFlags & InitSoundFlags.PersistInstance) == InitSoundFlags.PersistInstance;
        }

        protected static bool PersistRequested(PlaySoundFlags playSoundFlags)
        {
            return (playSoundFlags & PlaySoundFlags.PersistInstance) == PlaySoundFlags.PersistInstance;
        }

        protected static bool UnpersistRequested(StopSoundFlags stopSoundFlags)
        {
            return (stopSoundFlags & StopSoundFlags.UnsetPersistentFlag) == StopSoundFlags.UnsetPersistentFlag;
        }

        public static ISound CreateSound(ISoundDefinition soundDefinition)
        {
            return soundDefinition.CreateSound();
        }
    
        public static T CreateSound<T>(ISoundDefinition soundDefinition) where T : class, ISound<ISoundDefinition>
        {
            return soundDefinition.CreateSound<T>();
        }
        
        public static bool HasStopFlag(StopSoundFlags flagsToCheck, StopSoundFlags flag)
        {
            return (flagsToCheck & flag) == flag;
        }

        public static bool HasPlayFlag(PlaySoundFlags flagsToCheck, PlaySoundFlags flag)
        {
            return (flagsToCheck & flag) == flag;
        }

        protected static void SetSoundSourceObjectMultiple(IReadOnlyList<ISound> sounds, GameObject soundSourceObject)
        {
            for (int i = 0; i < sounds.Count; ++i)
            {
                sounds[i].SoundSourceObject = soundSourceObject;
            }
        }

        protected static void SetMidiNoteInfoMultiple(IReadOnlyList<ISound> sounds, MidiNoteInfo midiNoteInfo)
        {
            for (var i = 0; i < sounds.Count; ++i)
            {
                sounds[i].MidiNoteInfo = midiNoteInfo;
            }
        }
        
        protected static void UpdateSoundMultiple(IReadOnlyList<ISound> sounds) 
        {
            for (int i = 0; i < sounds.Count; ++i)
            {
                sounds[i].UpdateSound();
            }
        }

        protected static void RefreshSoundDefinitionMultiple(IReadOnlyList<ISound> sounds)
        {
            for (int i = 0; i < sounds.Count; ++i)
            {
                sounds[i].RefreshSoundDefinition();
            }
        }

        protected static void ReleaseResourcesMultiple(IReadOnlyList<ISound> sounds)
        {
            // Go backwards just in case any of the children are destroyed here.
            for (int i = sounds.Count - 1; i >= 0; --i)
            {
                sounds[i].ReleaseResources();
            }
        }

        protected static void StopMultiple(IReadOnlyList<ISound> sounds, StopSoundFlags stopSoundFlags)
        {
            for (int i = 0; i < sounds.Count; ++i)
            {
                sounds[i].Stop(stopSoundFlags);
            }
        }
        
        protected static void PauseMultiple(IReadOnlyList<ISound> sounds)
        {
            for (int i = 0; i < sounds.Count; ++i)
            {
                sounds[i].Pause();
            }
        }

        protected static void ResumeMultiple(IReadOnlyList<ISound> sounds)
        {
            for (int i = 0; i < sounds.Count; ++i)
            {
                sounds[i].Resume();
            }
        }

        protected static void MuteMultiple(IReadOnlyList<ISound> sounds)
        {
            for (int i = 0; i < sounds.Count; ++i)
            {
                sounds[i].Mute();
            }
        }
        
        protected static void UnmuteMultiple(IReadOnlyList<ISound> sounds)
        {
            for (int i = 0; i < sounds.Count; ++i)
            {
                sounds[i].Unmute();
            }
        }
        #endregion
    }
    
    
    /* - * - * - * - * - * - * - * - * - * - * - * - * - * - * - * - * - * - * - * - * - * - * - * - * - * - * - * - */
    /* - - - - - - - - - - - - - - - - - - - - - SOUND<TDefinition, TSelf> - - - - - - - - - - - - - - - - - - - - - */
    /* - * - * - * - * - * - * - * - * - * - * - * - * - * - * - * - * - * - * - * - * - * - * - * - * - * - * - * - */

    // TODO: Do we even need TSelf? Doesn't seem like it's being used anywhere.
    public abstract class Sound<TDefinition, TSelf> : Sound, ISound<ISoundDefinition>, ISoundInternal<TDefinition>
        where TDefinition : class, ISoundDefinition
        where TSelf : Sound
    {
        #region Private Events
        // TODO: Unsubscribe everything when we DeInit
        // protected event Action InitSoundModuleProcessor;
        // protected event Action SoundDefinitionChangedEvent;
        // protected event Action SoundUpdateTickEvent;
        // protected event Action UnityAudioGeneratorTickEvent;
        // protected event Action<PlaySoundFlags> BeforeChildPlayEvent;
        // protected event Action<PlaySoundFlags> BeforePlayEvent;
        #endregion
        
        #region ISound<ISoundDefinition> Properties
        public override ISoundDefinition SoundDefinition => _soundDefinition;
        protected TDefinition _soundDefinition;
        public override bool Pitched => _soundDefinition.Pitched;
        public override int BaseNoteNumber => _soundDefinition.BaseNoteNumber;
        public override Dictionary<Parameter, float> ParameterValues
        {
            get
            {
                if (HasParent)
                {
                    return ((ISoundInternal) ParentSound).ParameterValues;
                }
                else
                {
                    return _parameterValues;
                }
            }
        }
        #endregion

        #region Private Fields
        private BuiltInData _builtInData;
        private bool _initParametersComplete;
        private readonly Dictionary<Parameter, float> _parameterValues = new Dictionary<Parameter, float>();
        private readonly List<BuiltInParameter> _builtInParameters = new List<BuiltInParameter>();
        private readonly List<Parameter> _parameterList = new List<Parameter>();
        #endregion

        #region Constructor
        protected Sound()
        {
            _builtInData = BuiltInData.Instance;
            // BuiltInData.Instance.properties.GetSoundProperty(out _volumeProperty);
            // BuiltInData.Instance.properties.GetSoundProperty(out _pitchProperty);
            // BuiltInData.Instance.properties.GetSoundProperty(out _delayProperty);
            // BuiltInData.Instance.properties.GetSoundProperty(out _offsetProperty);
            // BuiltInData.Instance.properties.GetSoundProperty(out _timeBetweenProperty);
            // BuiltInData.Instance.properties.GetSoundProperty(out _repeatsProperty);
        }
        #endregion
        
        #region ISoundInternal<TDefinition> Methods
        void ISoundInternal<TDefinition>.Init(TDefinition soundDefinition, InitSoundFlags initSoundFlags)
        {
            // TODO: Handle all InitSoundFlags
            ResetStatus(SoundStatus.None);
            
            if (soundDefinition == null)
            {
                ResetStatus(SoundStatus.Invalid);
                Debug.LogError("HEAR XR: Missing sound definition.");
                return;
            }
            
            _soundDefinition = soundDefinition;
            
            _instancePlaybackInfo = new SoundInstancePlaybackInfo();
            
            PostSetSoundDefinition(initSoundFlags);
            
            if (!IsValid()) return;

            InitParameters(initSoundFlags);
            
            InitModules(initSoundFlags);
            PostInitModules(initSoundFlags);
            InvokeInitSoundModuleProcessor();
            
            ParseSoundDefinition();
            PostParseSoundDefinition();

            ApplySoundDefinitionAndProperties(initSoundFlags);
            if (!IsValid()) return;
            
            if (PersistRequested(initSoundFlags))
            {
                SetPersistent(true);
            }
            SetStatus(SoundStatus.Initialized, true);
            PostInit(initSoundFlags);
            if (!IsValid()) return;

            // ((ISoundInternal) this).PrepareToPlay();
        }
        #endregion

        #region Sound Abstract Methods
        public override void ReleaseResources()
        {
            if (IsPlayingOrTransitioning())
            {
                Debug.LogWarning($"HEAR XR: {this} Asked to release resources while still playing.");
            }
            DoReleaseResources();
        }

        public override void Play(PlaySoundFlags playSoundFlags = PlaySoundFlags.None)
        {
            // If we have MIDI note information...
            if (_midiNoteInfo != null)
            {
                // If we have a valid start time, change to scheduled sound.
                if (_midiNoteInfo.startTime > AudiobreadManager.INACTIVE_START_TIME)
                {
                    _instancePlaybackInfo.scheduledStart = true;
                    _instancePlaybackInfo.startTime = _midiNoteInfo.startTime;
                }
                
                // If we have valid duration, add that.
                if (_midiNoteInfo.duration > AudiobreadManager.INVALID_TIME_DURATION)
                {
                    _instancePlaybackInfo.scheduledEnd = true;
                    _instancePlaybackInfo.duration = _midiNoteInfo.duration;
                }
            }
            
            if (!IsReadyToPlay())
            {
                ((ISoundInternal) this).PrepareToPlay();
            }
            
            // TODO: Make sure all flags are accounted for.
            if (!CanPlay(playSoundFlags)) return;

            if (PersistRequested(playSoundFlags))
            {
                SetPersistent(true);
            }
            else
            {
                // Always register self, even if not persistent.
                RegisterSelf();
            }
            
            if (HasPlayFlag(playSoundFlags, PlaySoundFlags.PlayNext))
            {
                InvokeOnBeforeChildPlay(playSoundFlags);
            }
            else
            {
                InvokeOnBeforePlay(playSoundFlags);
            }

            if (_instancePlaybackInfo.startTime > AudiobreadManager.INACTIVE_START_TIME)
            {
                _instancePlaybackInfo.scheduledStart = true;
            }
            else
            {
                SetPlaybackState(PlaybackState.PlayInitiated);
            }
            
            DoPlay(playSoundFlags);
        }
        
        public override void PlayScheduled(double startTime, PlaySoundFlags playSoundFlags = PlaySoundFlags.None)
        {
            _instancePlaybackInfo.scheduledStart = true;
            _instancePlaybackInfo.startTime = startTime;
            
            // TODO: Roll this into Play() above. It's basically the same.
            if (!CanPlay(playSoundFlags)) return;

            if (HasPlayFlag(playSoundFlags, PlaySoundFlags.PlayNext))
            {
                InvokeOnBeforeChildPlay(playSoundFlags);
            }
            else
            {
                InvokeOnBeforePlay(playSoundFlags);
            }
            
            DoPlay(playSoundFlags);
        }

        // TODO: SoundModule hookup.
        internal override void AddSoundPropertyInfluence<T>(ValueContainer influence)
        {
            if (!_builtInData.properties.GetSoundProperty<T>(out var property))
            {
                Debug.LogError($"HEAR XR: Unable to find sound property {typeof(T).GetType()}");
                return;
            }
            AddSoundPropertyInfluence(property, influence);
        }
        
        internal override void AddSoundPropertyInfluence(SoundProperty property, ValueContainer influence)
        {
            Debug.Log($"INFL Attempt: {this} {property}");
            
            // TODO: SoundModule hookup.
            // if (_calculators.ContainsKey(property))
            // {
            //     Debug.Log($"INFL: {this} add influence to {property}");
            //     _calculators[property].AddInfluence(influence);
            //     Debug.Log($"Added {property.name} influence.");
            // }
            // else
            // {
            //     Debug.LogWarning($"Unable to add {property.name} influence, since this sound doesn't use it.");
            // }
        }

        internal override void RemoveSoundPropertyInfluence<T>(ValueContainer influence)
        {
            // TODO: SoundModule hookup.
            // if (!_builtInData.properties.GetSoundProperty<T>(out var property))
            // {
            //     Debug.LogError($"HEAR XR: Unable to find sound property {typeof(T).GetType()}");
            //     return;
            // }
            // if (_calculators.ContainsKey(property))
            // {
            //     _calculators[property].RemoveInfluence(influence);
            //     //Debug.Log($"Removed {property.name} influence.");
            // }
            // else
            // {
            //     Debug.LogWarning($"Unable to remove {property.name} influence, since this sound doesn't use it.");
            // }
        }
        
        // internal override void RemoveSoundPropertyInfluence(SoundProperty property, ValueContainer influence)
        // {
        //     if (_calculators.ContainsKey(property))
        //     {
        //         _calculators[property].RemoveInfluence(influence);
        //         Debug.Log($"Removed {property.name} influence.");
        //     }
        //     else
        //     {
        //         Debug.LogWarning($"Unable to remove {property.name} influence, since this sound doesn't use it.");
        //     }
        // }
        
        // NOTE: Moved to SoundModuleProcessor
        // protected override void RefreshCalculators()
        // {
        //     for (int i = 0; i < _soundProperties.Length; ++i)
        //     {
        //         _calculators[_soundProperties[i]].Generate();
        //         _calculators[_soundProperties[i]].Calculate();
        //     }
        // }
        // END

        protected override void DoRefreshSoundDefinition()
        {
            PostRefreshSoundDefinition();
            ParseSoundDefinition();
            PostParseSoundDefinition();
            
            InvokeSoundDefinitionChanged();
            // NOTE: Moved to SoundModuleProcessor
            // RefreshCalculators();
            // END
            
            ApplySoundDefinitionAndProperties();
        }

        public override TModuleDefinition GetModuleSoundDefinitions<TModuleDefinition>(SoundModule soundModule)
        {
            foreach (var soundModuleSoundDefinition in _soundDefinition.RuntimeSoundModuleDefinitions)
            {
                if (soundModuleSoundDefinition.soundModule == soundModule)
                {
                    return soundModuleSoundDefinition as TModuleDefinition;
                }
            }

            return null;
        }

        public override void SetParameter(Parameter parameter, float parameterValue)
        {
            if (parameter == null)
            {
                return;
            }

            // TODO: Validate input here
            if (_parameterValues.ContainsKey(parameter))
            {
                // TODO: Parameters should handle easing here.
                _parameterValues[parameter] = parameterValue;
            }
        }
        #endregion
        
        #region Abstract Methods
        protected abstract void DoPlay(PlaySoundFlags playFlags);
        #endregion
        
        #region Sound Method Overrides
        internal override void DoUpdate()
        {
            base.DoUpdate();
            
            // Update built-in parameter values.
            if (!HasParent)
            {
                UpdateAutoParameters();
            }
            
#if UNITY_EDITOR
            if (_soundDefinition.WasChanged)
            {
                RefreshSoundDefinition();
                _soundDefinition.WasChanged = false;
            }
#endif
            InvokeSoundUpdateTickEvent();
        }
        
        protected override bool CanStop(StopSoundFlags stopSoundFlags = StopSoundFlags.None)
        {
            // Deal with the unpersist flag first.
            if (UnpersistRequested(stopSoundFlags))
            {
                SetPersistent(false);
            }

            if (!base.CanStop(stopSoundFlags)) return false;

            if (HasStopFlag(stopSoundFlags, StopSoundFlags.Instant)) return true;

            if (_soundModuleGroupProcessor == null) return true;

            if (!_soundModuleGroupProcessor.CanStopNow(stopSoundFlags, out var blockingSoundModule))
            {
                _soundModuleGroupProcessor.RequestStop(stopSoundFlags, ref blockingSoundModule, HandleStopBlockReleased);
                return false;
            }
            return true;
        }

        protected override bool ShouldUpdate()
        {
            return base.ShouldUpdate() && !IsStoppedOrPaused();
        }

        protected override void OnDeInit()
        {
            // TODO: SoundModule hookup.
            // _soundProperties = null;
            // _soundPropertiesBySetType = null;
            // _calculators = null;
            _soundDefinition = null;
        }

        protected override void PostSetParent(ISound previousParent, ISound newParent)
        {
            base.PostSetParent(previousParent, newParent);
            
            // Propagate parameters to the parent.
            if (newParent == null) return;
            
            ((ISoundInternal) newParent).AddTrackedParameters(_parameterList);
            _parameterValues.Clear();
            _builtInParameters.Clear();
        }
        #endregion
        
        #region Overrideable Methods
        protected virtual void PostSetSoundDefinition(InitSoundFlags initSoundFlags = InitSoundFlags.None) {}
        protected virtual void PostInit(InitSoundFlags initSoundFlags) {}
        protected virtual void ParseSoundDefinition() {}
        protected virtual void PostParseSoundDefinition() {}
        protected virtual void PostRefreshSoundDefinition() {}
        protected virtual void ApplySoundDefinitionAndProperties(InitSoundFlags initSoundFlags = InitSoundFlags.None) {}
        protected virtual void PostInitSoundProperties(InitSoundFlags initSoundFlags = InitSoundFlags.None) {}
        
        // NOTE: Moved to SoundModuleProcessor
        // protected virtual void GatherSoundPropertyInfo(ref Dictionary<SoundProperty, Definition> soundPropertyInfo)
        // {
        //     soundPropertyInfo.Add(_volumeProperty, _soundDefinition.VolumeDefinition);
        // }
        // END Moved
        
        protected virtual void InitModules(InitSoundFlags initSoundFlags = InitSoundFlags.None)
        {
            _soundModuleGroupProcessor = new SoundModuleGroupProcessor(this);
            _hasSoundModuleGroupProcessor = true;
            SubscribeSoundModuleProcessorsToEvents();
        }

        protected virtual void PostInitModules(InitSoundFlags initSoundFlags = InitSoundFlags.None) {}

        protected virtual bool CanPlay(PlaySoundFlags playSoundFlags = PlaySoundFlags.None)
        {
            if (!IsValid() || (IsPlayingOrTransitioning() &&
                               HasPlayFlag(playSoundFlags, PlaySoundFlags.DoNotRetriggerIfPlaying))) return false;

            return IsReadyToPlay();
        }

        protected virtual void DoReleaseResources() {}
        
        public override void AddTrackedParameters(List<Parameter> parameters)
        {
            // If has parent, push it up.
            if (HasParent)
            {
                ((ISoundInternal) ParentSound).AddTrackedParameters(parameters);
                return;
            }
            
            // Otherwise, add the parameters here.
            for (var i = 0; i < parameters.Count; ++i)
            {
                if (!_parameterList.Contains(parameters[i]))
                {
                    _parameterList.Add(parameters[i]);
                }
            }

            InitParameterValues();
        }
        #endregion

        #region Private Methods
        private void InitParameters(InitSoundFlags initSoundFlags = InitSoundFlags.None)
        {
            if (!_initParametersComplete)
            {
                foreach (var soundModuleDefinition in _soundDefinition.RuntimeSoundModuleDefinitions)
                {
                    if (soundModuleDefinition.Parameters != null && soundModuleDefinition.Parameters.Count > 0)
                    {
                        for (var i = 0; i < soundModuleDefinition.Parameters.Count; ++i)
                        {
                            if (!_parameterList.Contains(soundModuleDefinition.Parameters[i].parameter))
                            {
                                _parameterList.Add(soundModuleDefinition.Parameters[i].parameter);
                            }
                        }
                    }
                }

                _initParametersComplete = true;
            }
            
            // If this sound has a parent, push the parameter list up.
            if (ParentSound != null)
            {
                ((ISoundInternal) ParentSound).AddTrackedParameters(_parameterList);
                return;
            }
            
            InitParameterValues();
        }

        private void InitParameterValues()
        {
            // TODO: This is a big and nasty bug. Somehow, on some sounds, the parameter values here are already set,
            //       and I have no idea how. This is causing all kinds of gross behavior. 
            //       As a hack, just clear the parameter values, but WHAT THE HELL?!
            _parameterValues.Clear();

            for (var i = 0; i < _parameterList.Count; ++i)
            {
                if (!_parameterValues.ContainsKey(_parameterList[i]))
                {
                    _parameterValues.Add(_parameterList[i], _parameterList[i].defaultValue);
                    if (_parameterList[i] is BuiltInParameter && !_builtInParameters.Contains(_parameterList[i]))
                    {
                        _builtInParameters.Add(_parameterList[i] as BuiltInParameter);
                    }
                }
            }
        }

        private void UpdateAutoParameters()
        {
            for (var i = 0; i < _builtInParameters.Count; ++i)
            {
                _parameterValues[_builtInParameters[i]] = _builtInParameters[i].Calculate(this, SetValuesType.OnUpdate);
            }
        }
        #endregion

        #region Helper Methods
        public override string ToString()
        {
            return _soundDefinition != null ? $"- SOUND - [{_soundDefinition.Name}] [GUID {Guid}]" : $"Sound {Guid}";
        }
        #endregion
        
        #region Event Handlers
        protected void HandleStopBlockReleased()
        {
            // Debug.Log($"{this} Stop block released");
            Stop(StopSoundFlags.Instant);
        }
        #endregion
    }
}