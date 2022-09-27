using System.Collections.Generic;
using UnityEngine;

namespace HearXR.Audiobread
{
    /// <summary>
    /// Sound Manager helps normalize the API across several audio middleware engines.
    /// </summary>
    public abstract class SoundManager : MonoBehaviour
    {
        #region Editor Fields
        [SerializeField] private bool _persistAcrossScenes = true;
        #endregion
        
        #region Properties
        /// <summary>
        /// The single instance of this class in the scene.
        /// </summary>
        public static SoundManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<SoundManager>();
                }
                return _instance;
            }
        }
        public SoundRegistry Registry => _soundRegistry;

        #endregion
        
        #region Private Fields
        protected static SoundManager _instance;
        protected List<ISound> _sounds;
        protected SoundRegistry _soundRegistry;
        #endregion
        
        #region Init
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }

            if (_persistAcrossScenes)
            {
                DontDestroyOnLoad(gameObject);
            }
            
            _soundRegistry = SoundRegistry.Instance;
            _sounds = _soundRegistry.Sounds;

            OnAwake();
        }
        #endregion

        #region Virtual Methods
        protected virtual void OnAwake() {}
        #endregion
    }   
}