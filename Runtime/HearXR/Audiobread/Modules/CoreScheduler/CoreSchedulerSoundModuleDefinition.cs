using System.Collections.Generic;
using HearXR.Audiobread.SoundProperties;
using UnityEngine;

namespace HearXR.Audiobread
{
    public class CoreSchedulerSoundModuleDefinition : SoundModuleDefinition, IChildSchedulerDefinition
    {
        [SerializeField] private RepeatsDefinition _repeatsDefinition;

        [SerializeField] private TimeBetweenDefinition _timeBetweenDefinition;
        
        // TODO: Maybe get rid of this? Parent repeats + random playback order might cause an issue.
        [SerializeField] private ParentSoundRepeatType _repeatType;
        
        internal Repeats RepeatsProperty
        {
            get
            {
                CacheProperties();
                return _repeatsProperty;
            }
        }

        internal TimeBetween TimeBetweenProperty
        {
            get
            {
                CacheProperties();
                return _timeBetweenProperty;
            }
        }

        public ParentSoundRepeatType RepeatType => _repeatType;

        private bool _propertiesCached;
        private Repeats _repeatsProperty;
        private TimeBetween _timeBetweenProperty;

        protected override void CacheProperties(ref Dictionary<SoundProperty, Definition> soundProperties)
        {
            CacheProperties();
            soundProperties.Add(_repeatsProperty, _repeatsDefinition);
            soundProperties.Add(_timeBetweenProperty, _timeBetweenDefinition);
        }
        
        private void CacheProperties()
        {
            if (_propertiesCached) return;
            _repeatsProperty = BuiltInData.Properties.GetSoundPropertyByType<Repeats>();
            _timeBetweenProperty = BuiltInData.Properties.GetSoundPropertyByType<TimeBetween>();
            _propertiesCached = true;
        }
    }
}