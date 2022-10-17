using System;
using System.Collections.Generic;
using UnityEngine;

namespace HearXR.Audiobread
{
    [ExecuteAlways]
    public class SoundRegistry
    {
        #region Events
        public static event Action<ISound> SoundRegisteredEvent;
        public static event Action<ISound> SoundUnregisteredEvent;
        #endregion
        
        #region Properties
        //public static SoundRegistry Instance => _instance ? _instance : (_instance = new SoundRegistry());
        public static SoundRegistry Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SoundRegistry();
                    //Debug.Log("HEAR XR: Created new Sound Registry with GUID: " + _instance.Guid);
                }

                return _instance;
            }
        }

        public List<ISound> Sounds => _sounds;
        public Guid Guid => _guid;
        #endregion

        #region Private Fields
        private static SoundRegistry _instance;
        private Guid _guid;
        protected List<ISound> _sounds = new List<ISound>();
        #endregion
        
        #region Constructor
        protected SoundRegistry()
        {
            _guid = Guid.NewGuid();
        }
        #endregion

        #region Internal Methods
        internal void RegisterSoundInstance(ISound sound)
        {
            _sounds.Add(sound);
            SoundRegisteredEvent?.Invoke(sound);
        }

        internal void UnregisterSoundInstance(ISound sound)
        {
            _sounds.Remove(sound);
            SoundUnregisteredEvent?.Invoke(sound);
        }
        
        internal void ClearRegistry()
        {
            _sounds.Clear();
        }
        #endregion
    }
}