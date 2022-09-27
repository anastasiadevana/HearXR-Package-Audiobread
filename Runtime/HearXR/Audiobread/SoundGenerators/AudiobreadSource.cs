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
        public enum AudioSourceMode
        {
            ClipPlayer,
            ToneGenerator,
            TestPlayer
        }
        #endregion
        
        #region Editor Fields
        [SerializeField] private AudioSource _audioSource;
        #endregion
        
        #region Properties
        public AudiobreadClip Player => _player;
        public TestAudiobreadClip TestPlayer => _testPlayer;
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
        private AudiobreadClip _player;
        private TestAudiobreadClip _testPlayer;
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

            if (_player == null)
            {
                _player = new AudiobreadClip(this);
            }

            if (_testPlayer == null)
            {
                _testPlayer = new TestAudiobreadClip(this);
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
                
                case AudioSourceMode.TestPlayer:
                    _testPlayer.PrepareToBeStolen();
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

        private float _timeIndex;
        
        #region Filter Read
//         private void OnAudioFilterRead(float[] buffer, int numChannels)
//         {
//             if (_audioSourceMode != AudioSourceMode.ToneGenerator) return;
//             
//             // Debug.Log("Yo");
//             _generator.OnAudioFilterRead(buffer, numChannels);
// /*
//             var _waveShape = ToneGenerator.WaveShape.Sin;
//             var _toneFrequency = 220f;
//             var _clipFrequency = 44100f;
//             var _squareVolumeFactor = 0.4f;
//             var _sawtoothVolumeFactor = 0.4f;
//
//             // Using direct generation methods.
//             for (var i = 0; i < buffer.Length; i += numChannels)
//             {
//                 switch (_waveShape)
//                 {
//                     case ToneGenerator.WaveShape.Sin:
//                         //buffer[i] = CreateSine(_timeIndex, _frequency, _sampleRate);
//                         buffer[i] = Mathf.Sin(Mathf.PI * 2.0f * _timeIndex * _toneFrequency / _clipFrequency);
//                         break;
//                 
//                     case ToneGenerator.WaveShape.Square:
//                         buffer[i] = ((Mathf.Repeat(_timeIndex * _toneFrequency / _clipFrequency,1) > 0.5f) ? 1.0f : -1.0f) * _squareVolumeFactor;
//                         break;
//                 
//                     case ToneGenerator.WaveShape.Sawtooth:
//                         buffer[i] = (Mathf.Repeat(_timeIndex * _toneFrequency / _clipFrequency,1) * 2.0f - 1.0f) * _sawtoothVolumeFactor;
//                         break;
//                 
//                     case ToneGenerator.WaveShape.Triangle:
//                         buffer[i] = Mathf.PingPong(_timeIndex * 2.0f * _toneFrequency / _clipFrequency,1) * 2.0f - 1.0f;
//                         break;
//                 
//                     default:
//                         throw new ArgumentOutOfRangeException();
//                 }
//
//                 for (var j = 1; j < numChannels; ++j)
//                 {
//                     buffer[i + j] = buffer[i];
//                 }
//             
//                 _timeIndex++;
//         
//                 // If timeIndex gets too big, reset it to 0
//                 // TODO: No magic numbers
//                 if (_timeIndex >= _clipFrequency * 2)
//                 {
//                     _timeIndex = 0;
//                 }
//             }
//             */
//         }
        #endregion
    }
}
