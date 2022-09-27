using HearXR.Audiobread.SoundProperties;
using UnityEngine;

namespace HearXR.Audiobread
{
    // TODO: Find all instances of Resources.Load of built-in data and get rid of them.
    // TODO: Rename this from BuiltInData to something else. Because it will handle more than built-in stuff.
    [ExecuteAlways]
    [CreateAssetMenu (menuName = "Audiobread/Built In Data")]
    public class BuiltInData : ScriptableObject
    {
        #region Editor Fields
        [SerializeField] private BuiltInSoundEventSet _events;
        [SerializeField] private BuiltInSoundPropertySet _properties;
        [SerializeField] private SoundModuleManager _soundModuleManager;
        #endregion
        
        #region Constants
        private const string BUILT_IN_DATA_PATH = "BuiltInData";
        #endregion

        #region Properties
        //public static BuiltInData Instance => _instance ? _instance : (_instance = Resources.Load<BuiltInData>(BUILT_IN_DATA_PATH));
        public static BuiltInData Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<BuiltInData>(BUILT_IN_DATA_PATH);
                    //_instance.properties.Init();
                }

                return _instance;
            }
            set => _instance = value;
        }

        /// <summary>
        /// Use this if you have a reference to an instantiated BuiltInData.
        /// </summary>
        public BuiltInSoundEventSet events => _events;
        public BuiltInSoundPropertySet properties => _properties;
        public SoundModuleManager soundModuleManager => _soundModuleManager;


        /// <summary>
        /// Static properties.
        /// </summary>
        public static BuiltInSoundEventSet Events => Instance._events;
        public static BuiltInSoundPropertySet Properties => Instance._properties;
        public static SoundModuleManager SoundModuleManager => Instance._soundModuleManager;
        #endregion
        
        #region Private Fields
        private static BuiltInData _instance;
        #endregion
    }
}