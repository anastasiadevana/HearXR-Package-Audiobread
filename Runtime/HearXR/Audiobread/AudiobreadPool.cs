using System.Collections.Generic;
using HearXR.Common;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HearXR.Audiobread
{
    [ExecuteAlways]
    public class AudiobreadPool
    {
        #region Delegates
        public delegate void PoolItemTemplateInitCallback(ref AudiobreadSource poolItemTemplate);
        #endregion
        
        #region Constants
        private const string PREFAB_PATH = "AudiobreadSoundSource";
        private const string GO_NAME = "TEMP_SOUND_POOL";
        #endregion
        
        #region Properties
        //public static AudiobreadPool Instance => _instance ? _instance : (_instance = new AudiobreadPool());
        public static AudiobreadPool Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AudiobreadPool();
                    //Debug.LogWarning("HEAR XR: Created new Audiobread Pool with ID " + _instance.Guid);
                }

                return _instance;
            }
        }
        public bool HasPool => _hasPool;

        public System.Guid Guid => _guid;
        #endregion

        #region Private Fields
        private static AudiobreadPool _instance;
        private ItemPool<AudiobreadSource> _audioSourcePool;
        private bool _hasPool;
        private System.Guid _guid;
        
#if UNITY_EDITOR
        private GameObject _editorPoolParent;
#endif
        #endregion
        
        #region Constructor
        protected AudiobreadPool()
        {
            _guid = System.Guid.NewGuid();
            // Just in case, do some cleaning.
#if UNITY_EDITOR
            // if (!Application.isPlaying)
            // {
            //     DestroyRogueObjects();
            // }
#endif
        }
        #endregion

        #region Internal Methods
        /// <summary>
        /// Initialize the audio source pool.
        /// If the pool is already initialized, this will do nothing.
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="size"></param>
        /// <param name="preloadWith"></param>
        /// <param name="parentTransform"></param>
        /// <param name="poolItemTemplateInitCallback"></param>
        internal void TryInitPool(int size, int preloadWith, Transform parentTransform, PoolItemTemplateInitCallback poolItemTemplateInitCallback = null)
        {
            if (_hasPool) return;
            var audiobreadSource = Resources.Load<AudiobreadSource>(PREFAB_PATH);
            
            var audiobreadSourceTemplate = audiobreadSource;
            
            // If some modules want to alter the template audio source pool item, clone the prefab first, so that
            // it doesn't get altered.
            if (poolItemTemplateInitCallback != null)
            {
                audiobreadSourceTemplate = Object.Instantiate(audiobreadSource, parentTransform);
                poolItemTemplateInitCallback?.Invoke(ref audiobreadSourceTemplate);
            }

            _audioSourcePool = new TypedItemPool<AudiobreadSource>(audiobreadSourceTemplate, size, preloadWith, parentTransform);
            _hasPool = true;
        }
        
#if UNITY_EDITOR
        /// <summary>
        /// Initialized the pool for non-runtime editor use.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="preloadWith"></param>
        internal void TryInitEditorPool(int size, int preloadWith)
        {
            //Debug.LogError("EDITOR INIT");
            if (_hasPool) return;
            AudiobreadSource audiobreadSource = Resources.Load<AudiobreadSource>(PREFAB_PATH);
            
            // Create a parent Game Object and hide it in Editor.
            _editorPoolParent = new GameObject(GO_NAME);
            
            //_editorPoolParent.hideFlags = HideFlags.HideAndDontSave;
            _audioSourcePool = new TypedItemPool<AudiobreadSource>(audiobreadSource, size, preloadWith, _editorPoolParent.transform);
            _hasPool = true;
        }

        internal void ClearEditorPool()
        {
            // TODO: We might want to go through the pool sources and call Steal on all of them first.
            if (_audioSourcePool != null)
            {
                _audioSourcePool.Clear();
            }
            Object.DestroyImmediate(_editorPoolParent);

            DestroyRogueObjects();
            _instance = null;
        }

        private void DestroyRogueObjects()
        {
            GameObject poolParent = null;
            do
            {
                if (poolParent != null)
                {
                    Object.DestroyImmediate(poolParent);
                }
                poolParent = GameObject.Find(GO_NAME);
            } while (poolParent != null);

            //GameObject[] poolParents = GameObject.Fin

            // _editorPoolParent = GameObject.Find(GO_NAME);
            // if (_editorPoolParent != null)
            // {
            //     Object.DestroyImmediate(_editorPoolParent);
            // }
            // AudiobreadSource[] rogueAudioSources = (AudiobreadSource[]) GameObject.FindObjectsOfType<AudiobreadSource>();
            // //Debug.LogError($"Audio sources found {rogueAudioSources.Length}");
            // for (int i = 0; i < rogueAudioSources.Length; ++i)
            // {
            //     Object.DestroyImmediate(rogueAudioSources[i].gameObject);
            // }
        }
#endif
        
        /// <summary>
        /// Get a new Audiobread Source for this sound definition.
        /// </summary>
        /// <param name="audiobreadSource">New Audiobread Source.</param>
        /// <returns>TRUE if was able to allocate a new source successfully. FALSE otherwise.</returns>
        internal bool TryGetAudioSource(out AudiobreadSource audiobreadSource)
        {
            return (_audioSourcePool.TryGetItem(out audiobreadSource, AudioSourceToStealIndexFinder));
        }

        internal void ReturnAudioSource(AudiobreadSource audiobreadSource)
        {
            _audioSourcePool.ReturnItem(audiobreadSource);
        }
        #endregion
        
        #region Private Methods
        /// <summary>
        /// Implementation of ItemPool.ItemToStealIndexFinder delegate.
        /// Chooses which Audio Source to steal.
        /// </summary>
        /// <param name="inUseAudioSources">List of audio sources currently in use.</param>
        /// <param name="index">Index of the audio source to steal.</param>
        /// <returns>TRUE if there was an available item to steal, FALSE otherwise.</returns>
        private bool AudioSourceToStealIndexFinder(in List<AudiobreadSource> inUseAudioSources, out int index)
        {
            //Debug.LogError("STEAL!");
            index = -1;

            // Use the following strategy to find the most applicable audio source to steal,
            // from most preferred to least preferred.
            // Oldest non-playing sound.
            // Oldest non-persistent sound.
            // Oldest non-looping sound.
            // Oldest sound of any kind.
            int oldestNonPlaying = -1;
            int oldestNonPersistent = -1;
            int oldestNonLooping = -1;
            int oldestSound = -1;

            for (int i = 0; i < inUseAudioSources.Count; ++i)
            {
                if (inUseAudioSources[i].Player.IsStopped())
                {
                    oldestNonPlaying = i;
                    break;
                }

                bool canGetParent = inUseAudioSources[i].Player.TryGetRegisteredParent(out ISound parentSound);
                
                //Debug.Log($"Pool attempt to get REGISTERED PARENT - success? {canGetParent}");
                
                if (canGetParent)
                {
                    //Debug.Log($"REGISTERED PARENT - we have a parent sound {parentSound.GetType().Name}");
                    if (!parentSound.IsPersistent())
                    {
                        oldestNonPersistent = i;
                        //break;
                    }

                    if (!parentSound.IsContinuous() && oldestNonLooping == -1)
                    {
                        oldestNonLooping = i; 
                    }
                }
                else
                {
                    Debug.Log($"MISSING PARENT SOUND on " + inUseAudioSources[i].GetHashCode());
                    return false;
                }
                
                if (oldestSound == -1)
                {
                    oldestSound = i;
                }
            }

            if (oldestNonPlaying != -1)
            {
                //Debug.Log("AUDIO POOL: Stealing oldest non-playing sound.");
                index = oldestNonPlaying;
            }
            else if (oldestNonPersistent != -1)
            {
                //Debug.Log("AUDIO POOL: Stealing oldest PLAYING non-persistent sound. Consider increasing the audio source pool size.");
                index = oldestNonPersistent;
            }
            else if (oldestNonLooping != -1)
            {
                //Debug.LogWarning("Stealing oldest non-continuous, but PLAYING and PERSISTENT sound. Consider increasing the audio source pool size.");
                index = oldestNonLooping;
            }
            else if (oldestSound != -1)
            {
                //Debug.LogWarning("Stealing oldest PLAYING, CONTINUOUS and PERSISTENT sound. Consider increasing the audio source pool size.");
                index = oldestSound;
            }
            else
            {
                // We should never get here #shifty-eyes.
            }

            return index > -1;
        }

        private void PreClearPoolItems(List<AudiobreadSource> inUseAudioSources, Queue<AudiobreadSource> pooledAudioSources)
        {
            // List<ISound> stoppedSounds = new List<ISound>();
            // for (int i = 0; i < inUseAudioSources.Count; ++i)
            // {
            //     ISound sound = inUseAudioSources[i].ParentSound;
            //     if (!stoppedSounds.Contains(sound))
            //     {
            //         sound.Stop(StopSoundFlags.Instant | StopSoundFlags.UnsetPersistentFlag);
            //         stoppedSounds.Add(sound);
            //     }
            //
            //     Object.DestroyImmediate(inUseAudioSources[i].gameObject);
            // }
            //
            // while (pooledAudioSources.Count > 0)
            // {
            //     AudiobreadSource source = pooledAudioSources.Dequeue();
            //     Object.DestroyImmediate(source.gameObject);
            // }
        }
        #endregion
    }
}

