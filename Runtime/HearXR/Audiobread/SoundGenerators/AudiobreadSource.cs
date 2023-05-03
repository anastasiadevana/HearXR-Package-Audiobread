using System;
using HearXR.Common;
using HearXR.Common.Pool;
using UnityEngine;

// TODO: It's a bit weird that AudiobreadSource is the pooled item.
//       The reason is that AudioClipPlayer is inheriting from the Sound class, but we also need a MonoBehaviour,
//       so that the sound can follow a GameObject. Ideally we would just have one class being responsible, but 
//       AudiobreadClipPlayer cannot inherit from both Sound and MonoBehaviour.
namespace HearXR.Audiobread
{
    [RequireComponent(typeof(AudioSource))]
    public class AudiobreadSource : SmoothFollower, IPoolItem
    {
        #region Data Structures
        // TODO: Right now these options are hard-coded, but we need to make this also modular.
        // TODO: Have some kind of interface that TestPlayer / ClipPlayer / ToneGenerator all implement.
        // TODO: Do some sort of dependency injection type thingy.
        public enum AudioSourceMode
        {
            ClipPlayer,
            ToneGenerator,
            Sampler
        }
        #endregion
        
        #region Editor Fields
        [SerializeField] private AudioSource _audioSource;
        #endregion
        
        #region Properties
        public SimpleSampler Sampler => _sampler;
        public AudiobreadClip Player => _player;
        public ToneGenerator Generator => _generator;
        
        public AudioSource AudioSource => _audioSource;

        public bool HasAudioSource => _hasAudioSource;

        public AudioSourceMode Mode
        {
            get => _audioSourceMode;
            set => _audioSourceMode = value;
        }
        #endregion
        
        #region Private Fields
        private AudioSourceMode _audioSourceMode;
        private SimpleSampler _sampler;
        private AudiobreadClip _player;
        private ToneGenerator _generator;
        private bool _hasAudioSource;
        private bool _init;
        #endregion
        
        #region Init
        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            if (_init) return;

            _hasAudioSource = ValidateFindOrCreateComponent(ref _audioSource);

            if (_sampler == null)
            {
                _sampler = new SimpleSampler(this);
            }
            
            if (_player == null)
            {
                _player = new AudiobreadClip(this);
            }

            if (_generator == null)
            {
                _generator = new ToneGenerator(this);
            }

            _init = true;
        }
        #endregion

        #region IPoolItem Methods
        public void PoolItemCreated()
        {
            Init();
        }

        public void PoolItemReturned()
        {
            ObjectToFollow = null;
        }

        public void PoolItemStolen()
        {
            switch (_audioSourceMode)
            {
                case AudioSourceMode.ClipPlayer:
                    _player.PrepareToBeStolen();
                    break;
                case AudioSourceMode.ToneGenerator:
                    _generator.PrepareToBeStolen();
                    break;
                case AudioSourceMode.Sampler:
                    _sampler.PrepareToBeStolen();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        #endregion
        
        #region Private Methods
        // TODO: Convert this to an extension or an editor drawer or something.
        private bool ValidateFindOrCreateComponent<T>(ref T component) where T : Component
        {
            if (component == null)
            {
                Debug.LogWarning($"HEAR XR: {typeof(T)} not connected.");
                component = gameObject.GetComponent<T>() as T;
                if (component == null)
                {
                    Debug.LogWarning($"HEAR XR: {typeof(T)} not found. This may cause performance issues at runtime.");
                    component = gameObject.AddComponent<T>() as T;
                }
            }

            return component != null;
        }
        #endregion
    }
}
