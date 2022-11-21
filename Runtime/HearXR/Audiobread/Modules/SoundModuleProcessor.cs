using System;
using System.Collections.Generic;
using HearXR.Audiobread.SoundProperties;
using UnityEngine;

// TODO: Refactor how sound properties work. Currently each module "owns" a sound property.
//       Instead we should separate. Something provides available sound properties on a sound, and other modules can
//       influence the value of that sound property.
//       For example, Pitch can be changed directly, or by setting a note number. Right now it's not possible because
//       the Core Unity module "owns" the Pitch property.
namespace HearXR.Audiobread
{
    /// <summary>
    /// An instance of SoundModuleProcessor is attached to each Sound instance that has that module enabled.
    /// This class manipulates the Sound instance based on the SoundDefinition.
    /// </summary>
    public abstract class SoundModuleProcessor : ISoundModuleProcessor
    {
        #region Private Fields
        private ISound _sound;
        protected Dictionary<SoundProperty, Calculator> _calculators;
        protected Dictionary<SetValuesType, SoundProperty[]> _soundPropertiesBySetType;
        protected SoundProperty[] _soundProperties;
        #endregion

        #region Properties
        public ISound MySound
        {
            get => _sound;
            protected set => _sound = value;
        }

        // TODO: I think there are other methods where we need to check this and bail out early.
        public bool Bypass { get; protected set; }
        #endregion
        
        #region Abstract Properties
        public abstract SoundModule SoundModule { get; }
        
        // TODO: Don't need GUID for this one, probably.
        public abstract Guid GUID { get; }
        #endregion

        internal bool TryGetSoundPropertyValueContainer(SoundProperty soundProperty, out ValueContainer valueContainer)
        {
            valueContainer = null;
            if (_calculators.ContainsKey(soundProperty))
            {
                valueContainer = _calculators[soundProperty].ValueContainer;
                return true;
            }
            return false;
        }
        
        #region Public Event Callbacks
        /// <summary>
        /// Module was initialized.
        /// </summary>
        internal void Init()
        {
            DoInit();
        }

        internal void HandleSoundUpdateTick(ref Sound.SoundInstancePlaybackInfo instancePlaybackInfo)
        {
            OnSoundUpdateTick(ref instancePlaybackInfo);
        }

        internal void HandleUnityAudioGeneratorTickEvent(ref Sound.SoundInstancePlaybackInfo instancePlaybackInfo)
        {
            OnUnityAudioGeneratorTick(ref instancePlaybackInfo);
        }

        internal void HandlePreparedToPlay(ref Sound.SoundInstancePlaybackInfo instancePlaybackInfo, PlaySoundFlags playSoundFlags = PlaySoundFlags.None)
        {
            OnPreparedToPlay(ref instancePlaybackInfo);
        }
        
        internal void HandleBeforeChildPlay(ref Sound.SoundInstancePlaybackInfo instancePlaybackInfo, PlaySoundFlags playSoundFlags)
        {
            OnBeforeChildPlay(ref instancePlaybackInfo, playSoundFlags);
        }

        internal void HandleBeforePlay(ref Sound.SoundInstancePlaybackInfo instancePlaybackInfo, PlaySoundFlags playSoundFlags)
        {
            OnBeforePlay(ref instancePlaybackInfo, playSoundFlags);
        }

        internal void HandleOnSoundBegan(ISound sound, double startTime)
        {
            // Debug.Log($"{sound} began");
            OnSoundBegan(sound, startTime);
        }

        internal void HandleOnSetParameter(Parameter parameter, float parameterValue)
        {
            OnSetParameter(parameter, parameterValue);
        }

        internal void HandleSoundDefinitionChanged()
        {
            OnSoundDefinitionChanged();
        }

        internal void HandleSetParent(ISound parentSound)
        {
            OnSetParent(parentSound);
        }
        
        // internal void HandleOnContainerBeforeFirstPlay()
        // {
        //     OnContainerBeforeFirstPlay();
        // }

        /// <summary>
        /// Apply sound definition to a Unity Audio Source.
        /// </summary>
        /// <param name="audiobreadSource"></param>
        internal void ApplySoundDefinitionToUnityAudio(AudiobreadSource audiobreadSource)
        {
            DoApplySoundDefinitionToUnityAudio(audiobreadSource);
        }
        #endregion

        #region Abstract Methods
        protected abstract void OnSoundDefinitionChanged();
        protected abstract void DoInit();
        protected abstract void OnSoundUpdateTick(ref Sound.SoundInstancePlaybackInfo instancePlaybackInfo);
        protected abstract void OnPreparedToPlay(ref Sound.SoundInstancePlaybackInfo instancePlaybackInfo);
        protected abstract void OnBeforeChildPlay(ref Sound.SoundInstancePlaybackInfo instancePlaybackInfo, PlaySoundFlags playSoundFlags);
        protected abstract void OnBeforePlay(ref Sound.SoundInstancePlaybackInfo instancePlaybackInfo, PlaySoundFlags playSoundFlags);
        protected abstract void OnSetParent(ISound parentSound);
        #endregion
        
        #region Virtual Methods
        protected virtual void DoApplySoundDefinitionToUnityAudio(AudiobreadSource audiobreadSource) {}
        protected virtual void OnUnityAudioGeneratorTick(ref Sound.SoundInstancePlaybackInfo instancePlaybackInfo) {}
        protected virtual void OnSoundBegan(ISound sound, double startTime) {}

        protected virtual void OnSetParameter(Parameter parameter, float parameterValue) {}
        // protected virtual void OnContainerBeforeFirstPlay() {}
        #endregion
    }
    
    public abstract class SoundModuleProcessor<TModule, TModuleDefinition> : SoundModuleProcessor
        where TModule : SoundModule
        where TModuleDefinition : SoundModuleDefinition
    {
        #region Private Fields
        private TModuleDefinition _moduleSoundDefinition;
        private TModule _soundModule;
        private Guid _guid;
        #endregion
        
        #region Properties
        public TModuleDefinition ModuleSoundDefinition => _moduleSoundDefinition;
        #endregion
        
        #region SoundModuleProcessor Abstract Properties
        public override Guid GUID => _guid;
        public override SoundModule SoundModule => _soundModule;
        #endregion

        #region Constructor
        public SoundModuleProcessor(TModule soundModule, ISound sound)
        {
            _soundModule = soundModule;
            _guid = Guid.NewGuid();
            MySound = sound;
        }
        #endregion

        protected Dictionary<Parameter, float> _parameters = new Dictionary<Parameter, float>();

        protected override void OnSetParameter(Parameter parameter, float parameterValue)
        {
            // Debug.Log($"Handle on set parameter!!! {parameter} {this}");
            
            // Debug.Log($"{_parameters.Count} {this}");
            
            base.OnSetParameter(parameter, parameterValue);
            if (_parameters.ContainsKey(parameter))
            {
                // TODO: SoundParameterDefinition should handle easing here.
                _parameters[parameter] = parameterValue;
                // Debug.Log($"Set parameter {parameter} to {parameterValue} on {this}");
                // Debug.Log($"{parameterValue}");
            }
        }

        protected override void DoInit()
        {
            _moduleSoundDefinition = MySound.GetModuleSoundDefinitions<TModuleDefinition>(SoundModule);
            InitCalculators();

            // Add parameters
            
            if (MySound.SoundDefinition.Parameters != null && MySound.SoundDefinition.Parameters.Count > 0)
            {
                for (int i = 0; i < MySound.SoundDefinition.Parameters.Count; ++i)
                {
                    if (_calculators.ContainsKey(MySound.SoundDefinition.Parameters[i].soundProperty))
                    {
                        if (!_parameters.ContainsKey(MySound.SoundDefinition.Parameters[i].parameter))
                        {
                            _parameters.Add(MySound.SoundDefinition.Parameters[i].parameter,
                                MySound.SoundDefinition.Parameters[i].parameter.defaultValue);
                            
                            _calculators[MySound.SoundDefinition.Parameters[i].soundProperty]
                                .AddParameterDefinition(MySound.SoundDefinition.Parameters[i]);
                            
                            // Debug.Log($"Added parameter {MySound.SoundDefinition.Parameters[i].parameter} to {this}");
                        }
                    }
                }
            }

            // if (_parameters.Count > 0)
            // {
            //     Debug.Log($"{_parameters.Count} {this}");   
            // }

            PostInitCalculators();
        }

        protected override void OnSoundDefinitionChanged()
        {
            RegenerateCalculators(SetValuesType.All);
        }

        protected override void OnSoundUpdateTick(ref Sound.SoundInstancePlaybackInfo instancePlaybackInfo)
        {
            CalculateCalculators(SetValuesType.OnUpdate);
        }

        protected override void OnSetParent(ISound parentSound)
        {
            if (_calculators.Count == 0) return;

            var processor = ((ISoundInternal) parentSound).SoundModuleGroupProcessor;

            // Try to find a matching sound module with the parent.
            if (!((ISoundInternal) parentSound).SoundModuleGroupProcessor.TryGetMatchingSoundModuleProcessor(
                _soundModule,
                out var parentSoundModuleProcessor)) return;

            for (var i = 0; i < _soundProperties.Length; ++i)
            {
                if (!_soundProperties[i].InfluenceChildNodes) continue;
                
                if (!parentSoundModuleProcessor.TryGetSoundPropertyValueContainer(_soundProperties[i],
                    out var parentValueContainer)) continue;
                
                _calculators[_soundProperties[i]].AddInfluence(parentValueContainer);
            }
        }

        protected override void OnPreparedToPlay(ref Sound.SoundInstancePlaybackInfo instancePlaybackInfo)
        {
            RegenerateCalculators(SetValuesType.OnPreparedToPlay);
            if (Bypass) return;
            
            ApplySoundModifiers(ref instancePlaybackInfo, SetValuesType.OnPreparedToPlay);
        }

        protected override void OnBeforeChildPlay(ref Sound.SoundInstancePlaybackInfo instancePlaybackInfo, PlaySoundFlags playSoundFlags)
        {
            RegenerateCalculators(SetValuesType.OnBeforeChildPlay, playSoundFlags);
            if (Bypass) return;
            
            ApplySoundModifiers(ref instancePlaybackInfo, SetValuesType.OnBeforeChildPlay, playSoundFlags);
        }

        protected override void OnBeforePlay(ref Sound.SoundInstancePlaybackInfo instancePlaybackInfo, PlaySoundFlags playSoundFlags)
        {
            RegenerateCalculators(SetValuesType.OnBeforePlay, playSoundFlags);
            if (Bypass) return;
            
            ApplySoundModifiers(ref instancePlaybackInfo, SetValuesType.OnBeforePlay, playSoundFlags);
        }

        private void InitCalculators()
        {
            // TODO: Instead of regenerating these values (especially for the AudiobreadClip) every single time, instead 
            //       instantiate each sound with the dictionaries already allocated for all known properties.
            //       Then just flip TRUE / FALSE for when this property is in use.
            
            var soundPropertyInfo = _moduleSoundDefinition.GetSoundProperties();
            
            // Debug.Log($"{this}: {MySound} has {soundPropertyInfo.Count} sound properties");
            
            // TODO: Some of these things can be made static per sub-module.

            // Generate a calculator for each sound property.
            _calculators = new Dictionary<SoundProperty, Calculator>();
            _soundProperties = new SoundProperty[soundPropertyInfo.Count];

            var i = 0;
            foreach (var item in soundPropertyInfo)
            {
                // Debug.Log($"InitCalculators(): {item.Key.ShortName} {item.Key.GetInstanceID()} - HAVE", item.Key);
                
                var prop = item.Key;
                _soundProperties[i] = prop;
                _calculators.Add(prop, prop.CreateCalculator());
                _calculators[prop].SetDefinition(item.Value);
                ++i;
            }
            
            // Organize sound properties by set type.
            _soundPropertiesBySetType = new Dictionary<SetValuesType, SoundProperty[]>();
            
            foreach (var item in BuiltInData.Properties.PropertiesBySetType)
            {
                var propsBySetType = new List<SoundProperty>();
                
                for (i = 0; i < item.Value.Length; ++i)
                {
                    if (soundPropertyInfo.ContainsKey(item.Value[i]))
                    {
                        // Debug.Log($"{item.Key} {item.Value[i].ShortName} {item.Value[i].GetInstanceID()} - YES!", item.Value[i]);
                        propsBySetType.Add(item.Value[i]);
                    }
                }
                _soundPropertiesBySetType.Add(item.Key, propsBySetType.ToArray()); 
            }
        }

        private void RegenerateCalculators(SetValuesType setValuesType, PlaySoundFlags playSoundFlags = PlaySoundFlags.None)
        {
            if (!MySound.IsValid() || setValuesType == SetValuesType.OnUpdate) return;

            // Debug.Log(setValuesType);
            var properties = setValuesType == SetValuesType.All ? _soundProperties : _soundPropertiesBySetType[setValuesType];

            // var debug = SoundModule.DisplayName == "Core Scheduler" && setValuesType == SetValuesType.OnBeforeChildPlay;

            for (int i = 0; i < properties.Length; ++i)
            {
                if (!_calculators.ContainsKey(properties[i]))
                {
                    Debug.LogError($"HEAR XR {this} doesn't have the calculator for {properties[i].name}");
                    continue;
                }

                // if (debug)
                // {
                //     Debug.Log($"{properties[i]} before: {_calculators[properties[i]].ValueContainer.FloatValue}");
                // }
                
                _calculators[properties[i]].Generate();
                _calculators[properties[i]].Calculate(ref _parameters);
                
                // if (debug)
                // {
                //     Debug.Log($"{properties[i]} after: {_calculators[properties[i]].ValueContainer.FloatValue}");
                // }
            }
        }
        
        private void CalculateCalculators(SetValuesType setValuesType, PlaySoundFlags playSoundFlags = PlaySoundFlags.None)
        {
            if (!MySound.IsValid()) return;
            
            var properties = _soundPropertiesBySetType[setValuesType];
            
            for (int i = 0; i < properties.Length; ++i)
            {
                if (!_calculators.ContainsKey(properties[i]))
                {
                    Debug.LogError($"HEAR XR {this} doesn't have the calculator for {properties[i].name}");
                }
                _calculators[properties[i]].Calculate(ref _parameters);
            }
        }

        protected abstract void ApplySoundModifiers(ref Sound.SoundInstancePlaybackInfo instancePlaybackInfo, SetValuesType setValuesType, 
            PlaySoundFlags playSoundFlags = PlaySoundFlags.None);
        
        protected virtual void PostInitCalculators() {}
    }
}