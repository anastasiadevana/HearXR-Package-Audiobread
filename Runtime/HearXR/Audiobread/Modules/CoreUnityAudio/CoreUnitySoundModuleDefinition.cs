using System.Collections.Generic;
using HearXR.Audiobread.SoundProperties;
using UnityEngine;

namespace HearXR.Audiobread.Core
{
    public class CoreUnitySoundModuleDefinition : SoundModuleDefinition
    {
        #region Static Private Fields
        private static bool _propertiesCached;
        private static Pitch _pitchProperty;
        private static NoteNumber _noteNumberProperty;
        private static Volume _volumeProperty;
        private static Delay _delayProperty;
        private static TimeDuration _timeDurationProperty;
        #endregion
        
        #region Static Properties
        public static Pitch PitchProperty
        {
            get
            {
                CacheProperties();
                return _pitchProperty;
            }
        }
        
        public static NoteNumber NoteNumberProperty
        {
            get
            {
                CacheProperties();
                return _noteNumberProperty;
            }
        }

        public static Volume VolumeProperty
        {
            get
            {
                CacheProperties();
                return _volumeProperty;
            }
        }

        public static Delay DelayProperty
        {
            get
            {
                CacheProperties();
                return _delayProperty;
            }
        }
        
        public static TimeDuration TimeDurationProperty
        {
            get
            {
                CacheProperties();
                return _timeDurationProperty;
            }
        }
        #endregion
        
        public PitchDefinition pitchPropertyDefinition;
        public NoteNumberDefinition noteNumber;
        public VolumeDefinition volumePropertyDefinition;
        public DelayDefinition delayPropertyDefinition;
        public TimeDurationDefinition durationPropertyDefinition;
        
        [Header("Fade In"), FadeValues(typeof(Volume), Fade.Direction.In)] public Fade fadeInDefinition;
        [Header("Fade Out"), FadeValues(typeof(Volume), Fade.Direction.Out)] public Fade fadeOutDefinition;

        protected override void CacheProperties(ref Dictionary<SoundProperty, Definition> soundProperties)
        {
            CacheProperties();
            soundProperties.Add(_pitchProperty, pitchPropertyDefinition);
            soundProperties.Add(_noteNumberProperty, noteNumber);
            soundProperties.Add(_volumeProperty, volumePropertyDefinition);
            soundProperties.Add(_delayProperty, delayPropertyDefinition);
            soundProperties.Add(_timeDurationProperty, durationPropertyDefinition);
        }
        
        #region Private Static Methods
        private static void CacheProperties()
        {
            if (_propertiesCached) return;
            _pitchProperty = BuiltInData.Properties.GetSoundPropertyByType<Pitch>();
            _noteNumberProperty = BuiltInData.Properties.GetSoundPropertyByType<NoteNumber>();
            _volumeProperty = BuiltInData.Properties.GetSoundPropertyByType<Volume>();
            _delayProperty = BuiltInData.Properties.GetSoundPropertyByType<Delay>();
            _timeDurationProperty = BuiltInData.Properties.GetSoundPropertyByType<TimeDuration>();
            _propertiesCached = true;
        }
        #endregion
    }
}