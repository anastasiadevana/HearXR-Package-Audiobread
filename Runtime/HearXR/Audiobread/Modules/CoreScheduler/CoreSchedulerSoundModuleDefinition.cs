using System.Collections.Generic;
using HearXR.Audiobread.SoundProperties;
using UnityEngine;

namespace HearXR.Audiobread
{
    public class CoreSchedulerSoundModuleDefinition : SoundModuleDefinition, IChildSchedulerDefinition
    {
        #region Editor Fields
        [SerializeField] private RepeatsDefinition _repeatsDefinition;
        [SerializeField] private TimeBetweenDefinition _timeBetweenDefinition;
        // TODO: Maybe get rid of this? Parent repeats + random playback order might cause an issue.
        [SerializeField] private ParentSoundRepeatType _repeatType;
        #endregion
        
        #region Static Private Fields
        private static bool _propertiesCached;
        private static Repeats _repeatsProperty;
        private static TimeBetween _timeBetweenProperty;
        #endregion
        
        #region Static Properties
        public static Repeats RepeatsProperty
        {
            get
            {
                CacheProperties();
                return _repeatsProperty;
            }
        }

        public static TimeBetween TimeBetweenProperty
        {
            get
            {
                CacheProperties();
                return _timeBetweenProperty;
            }
        }
        #endregion

        #region Properties
        public ParentSoundRepeatType RepeatType => _repeatType;
        #endregion

        protected override void CacheProperties(ref Dictionary<SoundProperty, Definition> soundProperties)
        {
            CacheProperties();
            soundProperties.Add(_repeatsProperty, _repeatsDefinition);
            soundProperties.Add(_timeBetweenProperty, _timeBetweenDefinition);
        }
        
        #region Private Static Methods
        private static void CacheProperties()
        {
            if (_propertiesCached) return;
            _repeatsProperty = BuiltInData.Properties.GetSoundPropertyByType<Repeats>();
            _timeBetweenProperty = BuiltInData.Properties.GetSoundPropertyByType<TimeBetween>();
            _propertiesCached = true;
        }
        #endregion
    }
}