using System.Collections.Generic;
using UnityEngine;

namespace HearXR.Audiobread
{
    public class Audiobread : SoundManager
    {
        #region Editor Fields
        [SerializeField] private int _audioSourcePoolSize = 20;
        [SerializeField] private int _preloadPoolWith = 20;
        #endregion
        
        #region Properties
        // TODO: This override below doesn't do a null check.
        public new static Audiobread Instance => SoundManager.Instance as Audiobread;
        public AudiobreadPool Pool => _audiobreadPool;
        #endregion
        
        #region Private Fields
        private AudiobreadPool _audiobreadPool;
        //private BuiltInData _builtInData;
        #endregion
                
        #region Init
        protected override void OnAwake()
        {
            //_builtInData = BuiltInData.Instance;
            _audiobreadPool = AudiobreadPool.Instance;
            if (!_audiobreadPool.HasPool)
            {
                _audiobreadPool.TryInitPool(_audioSourcePoolSize, _preloadPoolWith, transform, BuiltInData.SoundModuleManager.InitPoolItemTemplate);
            }
        }
        #endregion
        
        #region Loop
        private void Update()
        {
            // TODO: Make sure that _sounds survive assembly reloads!
            if (_sounds == null)
            {
                return;
            }
            
            // Keeping the update loop centralized for better control of the script execution order and allegedly
            // some performance gains.
            for (int i = _sounds.Count - 1; i >= 0; --i)
            {
                _sounds[i].UpdateSound();
            }
        }
        #endregion
        
        #region Public Methods
        public ISound CreateSound(ISoundDefinition soundDefinition, InitSoundFlags initFlags = InitSoundFlags.None)
        {
            return soundDefinition.CreateSound(initFlags);
        }

        public ISound PlaySound(ISoundDefinition soundDefinition, PlaySoundFlags playFlags = PlaySoundFlags.None)
        {
            var sound = soundDefinition.CreateSound();
            sound.Play(playFlags);
            return sound;
        }
        
        /// <summary>
        /// Stop all sounds that matched the passed in arguments.
        /// If multiple arguments have been passed in, only the sounds that meet all of the requirements will be stopped.
        /// If no arguments have been passed in, this will stop all sounds.
        /// </summary>
        /// <param name="soundDefinition">Stop all sounds of this sound definition.</param>
        /// <param name="soundSourceObject">Stop all sounds following this game object.</param>
        public void StopSounds(ISoundDefinition soundDefinition = null, GameObject soundSourceObject = null, StopSoundFlags flags = StopSoundFlags.None)
        {
            // TODO: Set up a couple of convenience overloads for this function.
            // TODO: Handle flags.
            // TODO: Add ability to override fades.
            // TODO: I really don't like that someone can stop persistent sounds and NOT have a reference to the instance.
            //       This will cause orphaned references to be hanging around.
            //       Maybe this function defaults to unsetting all persistence flags.
            
            // TODO: Iterate BACKWARDS! (doh)
            // Do not stop sounds while we're iterating, since that can screw up the iterator.
            List<ISound> stopThese = new List<ISound>();
            for (int i = 0; i < _sounds.Count; ++i)
            {
                if (soundDefinition != null)
                {
                    // TODO: !!!!!!!!!
                    //if (_sounds[i].SoundDefinition.GetInstanceID() != soundDefinition.GetInstanceID()) { continue; }
                }
                
                if (soundSourceObject != null)
                {
                    // TODO: !!!!!!!!!
                    //if (_sounds[i].SoundSourceObject.GetInstanceID() != soundSourceObject.GetInstanceID()) { continue; }
                }

                stopThese.Add(_sounds[i]);
            }

            for (int i = 0; i < stopThese.Count; ++i)
            {
                stopThese[i].Stop(flags);
            }
        }
        #endregion
        
        #region Static Internal Methods
        internal static float GetClampedRandomValue(float baseValue, float variance, float min, float max)
        {
            // TODO: This should go in Common.
            float value = (variance > 0.0f) ? UnityEngine.Random.Range(baseValue - variance, baseValue + variance) : baseValue;
            return Mathf.Clamp(value, min, max);
        }
        
        internal static int GetClampedRandomValue(int baseValue, int variance, int min, int max)
        {
            // TODO: This should go in Common.
            int value = (variance > 0) ? UnityEngine.Random.Range(baseValue - variance, baseValue + variance + 1) : baseValue;
            //Debug.LogWarning($"Generated random INT {value} between {baseValue - variance} and {baseValue + variance + 1}");
            return Mathf.Clamp(value, min, max);
        }
        
        internal static PlaybackState GetPlaybackStateFromChildren(PlaybackState parentState, IReadOnlyList<ISound> children, int nonStoppedChildrenCount)
        {
            bool anyPlayInitiated = false;
            bool anyStopInitiated = false;
            bool anyPlaying = false;
            
            for (int i = 0; i < children.Count; ++i)
            {
                switch (children[i].PlaybackState)
                {
                    case PlaybackState.Playing:
                        anyPlaying = true;
                        break;

                    case PlaybackState.PlayInitiated:
                        anyPlayInitiated = true;
                        break;

                    case PlaybackState.StopInitiated:
                        anyStopInitiated = true;
                        break;
                }
            }

            // Parent playback state is affected by the states of children.
            // We can only move forward in the playback state as such:
            // Stopped > PlayInitiated > Playing > StopInitiated
            // Ignore previous and current states.

            // TODO: If I made PlaybackState Flag, I could probably do some clever bitwise stuff here, but eh.
            switch (parentState)
            {
                case PlaybackState.Stopped:
                    if (anyPlayInitiated) return PlaybackState.PlayInitiated;
                    if (anyPlaying) return PlaybackState.Playing;
                    if (anyStopInitiated) return PlaybackState.StopInitiated;
                    break;

                case PlaybackState.PlayInitiated:
                    if (anyPlayInitiated) return parentState;
                    if (anyPlaying) return PlaybackState.Playing;
                    if (anyStopInitiated) return PlaybackState.StopInitiated;
                    break;

                case PlaybackState.Playing:
                    if (anyPlayInitiated || anyPlaying) return parentState;
                    if (anyStopInitiated) return PlaybackState.StopInitiated;
                    break;

                case PlaybackState.StopInitiated:
                    if (anyPlaying || anyPlayInitiated || anyStopInitiated) return parentState;
                    break;
            }

            // Set to Stopped as the default option.
            if (nonStoppedChildrenCount > 0)
            {
                Debug.LogError($"HEAR XR: Number of playing or transitioning players " +
                               $"{nonStoppedChildrenCount} doesn't match playback state!");
                return parentState;
            }

            return PlaybackState.Stopped;
        }
        #endregion
    }
}