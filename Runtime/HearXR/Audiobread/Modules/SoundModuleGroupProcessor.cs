using System;
using System.Collections.Generic;
using UnityEngine;

namespace HearXR.Audiobread
{
    public class SoundModuleGroupProcessor
    {
        #region Constructor
        internal SoundModuleGroupProcessor(ISound sound)
        {
            _sound = sound;
            
            foreach (var soundModuleSoundDefinitions in sound.SoundDefinition.ModuleSoundDefinitions)
            {
                var moduleProcessor = soundModuleSoundDefinitions.soundModule.CreateSoundModuleProcessor(sound);

                if (moduleProcessor is IStopControllerProcessor)
                {
                    _hasStopControllers = true;
                    _stopControllers.Add(moduleProcessor);
                }
                
                // TODO:
                SubscribeSoundModuleProcessorToEvents(ref moduleProcessor);

                _soundModuleProcessors.Add(moduleProcessor);
                // Debug.Log($"Add module {moduleProcessor.GetType()} to {_sound}");
            }
            
            // Debug.Log($"{_sound} sound has {_soundModuleProcessors.Count} modules");
        }
        #endregion

        #region Private Fields
        private ISound _sound;
        private readonly List<SoundModuleProcessor> _soundModuleProcessors = new List<SoundModuleProcessor>();
        private bool _hasStopControllers;
        private List<SoundModuleProcessor> _stopControllers = new List<SoundModuleProcessor>();
        #endregion

        #region Internal Methods
        internal bool CanStopNow(StopSoundFlags stopSoundFlags, out IStopControllerProcessor blockingProcessor)
        {
            blockingProcessor = null;
            if (!_hasStopControllers) return true;
            for (int i = 0; i < _stopControllers.Count; ++i)
            {
                if (_stopControllers[i] is IStopControllerProcessor checkController && !checkController.CanStopNow(stopSoundFlags))
                {
                    blockingProcessor = checkController;
                    return false;
                }
            }
            return true;
        }

        internal void RequestStop(StopSoundFlags stopSoundFlags, ref IStopControllerProcessor blockingProcessor,
            StopControllerStopCallback stopCallback)
        {
            blockingProcessor.RequestStop(stopSoundFlags, stopCallback);
        }

        internal bool TryGetMatchingSoundModuleProcessor(in SoundModule referenceSoundModule,
            out SoundModuleProcessor matchingSoundModuleProcessor)
        {
            matchingSoundModuleProcessor = null;
        
            for (int i = 0; i < _soundModuleProcessors.Count; ++i)
            {
                if (_soundModuleProcessors[i].SoundModule == referenceSoundModule)
                {
                    matchingSoundModuleProcessor = _soundModuleProcessors[i];
                    return true;
                }
            }

            return false;
        }

        internal bool TryGetProcessor<T>(out T soundModuleProcessor) where T : SoundModuleProcessor
        {
            soundModuleProcessor = null;
            {
                for (int i = 0; i < _soundModuleProcessors.Count; ++i)
                {
                    if (_soundModuleProcessors[i] is T)
                    {
                        soundModuleProcessor = _soundModuleProcessors[i] as T;
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

        #region Event Handling and Propagation
        private void SubscribeSoundModuleProcessorToEvents(ref SoundModuleProcessor moduleProcessor)
        {
            InitSoundModuleProcessor += moduleProcessor.Init;
            SoundDefinitionChangedEvent += moduleProcessor.HandleSoundDefinitionChanged;
            SoundUpdateTickEvent += moduleProcessor.HandleSoundUpdateTick;
            UnityAudioGeneratorTickEvent += moduleProcessor.HandleUnityAudioGeneratorTickEvent;
            BeforeChildPlayEvent += moduleProcessor.HandleBeforeChildPlay;
            BeforePlayEvent += moduleProcessor.HandleBeforePlay;
            ParentSetEvent += moduleProcessor.HandleSetParent;
            OnSoundBegan += moduleProcessor.HandleOnSoundBegan;
            ApplySoundDefinitionToUnityAudio += moduleProcessor.ApplySoundDefinitionToUnityAudio;
            // OnContainerBeforeFirstPlay += moduleProcessor.HandleOnContainerBeforeFirstPlay;
        }
        
        private void UnsubscribeSoundModuleProcessorFromEvents(ref SoundModuleProcessor moduleProcessor)
        {
            // TODO: When should we call this?
            InitSoundModuleProcessor -= moduleProcessor.Init;
            SoundDefinitionChangedEvent -= moduleProcessor.HandleSoundDefinitionChanged;
            SoundUpdateTickEvent -= moduleProcessor.HandleSoundUpdateTick;
            UnityAudioGeneratorTickEvent -= moduleProcessor.HandleUnityAudioGeneratorTickEvent;
            BeforeChildPlayEvent -= moduleProcessor.HandleBeforeChildPlay;
            BeforePlayEvent -= moduleProcessor.HandleBeforePlay;
            ParentSetEvent -= moduleProcessor.HandleSetParent;
            OnSoundBegan -= moduleProcessor.HandleOnSoundBegan;
            ApplySoundDefinitionToUnityAudio -= moduleProcessor.ApplySoundDefinitionToUnityAudio;
            // OnContainerBeforeFirstPlay -= moduleProcessor.HandleOnContainerBeforeFirstPlay;
        }
        
        private event Action InitSoundModuleProcessor;
        internal void Init()
        {
            InitSoundModuleProcessor?.Invoke();
        }

        private event Action SoundDefinitionChangedEvent;
        internal void HandleSoundDefinitionChanged()
        {
            SoundDefinitionChangedEvent?.Invoke();
        }

        private event Action SoundUpdateTickEvent;
        internal void HandleSoundUpdateTick()
        {
            SoundUpdateTickEvent?.Invoke();
        }
        
        private event Action UnityAudioGeneratorTickEvent;
        internal void HandleUnityAudioGeneratorTickEvent()
        {
            UnityAudioGeneratorTickEvent?.Invoke();
        }
        
        private event Sound.BeforePlayAction BeforeChildPlayEvent;
        internal double HandleBeforeChildPlay(PlaySoundFlags playSoundFlags, double startTime)
        {
            return BeforeChildPlayEvent?.Invoke(playSoundFlags, startTime) ?? startTime;
        }
        
        private event Sound.BeforePlayAction BeforePlayEvent;
        internal double HandleBeforePlay(PlaySoundFlags playSoundFlags, double startTime)
        {
            return BeforePlayEvent?.Invoke(playSoundFlags, startTime) ?? startTime;
        }
        
        private event Action<ISound> ParentSetEvent;
        internal void HandleSetParent(ISound parentSound)
        {
            ParentSetEvent?.Invoke(parentSound);
        }
        
        private event Action<ISound, double> OnSoundBegan;
        internal void HandleOnSoundBegan(ISound sound, double startTime)
        {
            Debug.Log("Sound began");
            
            OnSoundBegan?.Invoke(sound, startTime);
        }

        protected event Action<AudiobreadSource> ApplySoundDefinitionToUnityAudio;
        internal void HandleApplySoundDefinitionToUnityAudio(AudiobreadSource audiobreadSource)
        {
            ApplySoundDefinitionToUnityAudio?.Invoke(audiobreadSource);
        }

        // public event Action OnContainerBeforeFirstPlay;
        // internal void HandleOnContainerBeforeFirstPlay()
        // {
        //     OnContainerBeforeFirstPlay?.Invoke();
        // }
        #endregion
    }
}