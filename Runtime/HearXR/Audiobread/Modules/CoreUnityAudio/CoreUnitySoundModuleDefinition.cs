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
        private static Volume _volumeProperty;
        private static Delay _delayProperty;
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
        #endregion
        
        public PitchDefinition pitchPropertyDefinition;
        public VolumeDefinition volumePropertyDefinition;
        public DelayDefinition delayPropertyDefinition;
        
        [Header("Fade In"), FadeValues(typeof(Volume), Fade.Direction.In)] public Fade fadeInDefinition;
        [Header("Fade Out"), FadeValues(typeof(Volume), Fade.Direction.Out)] public Fade fadeOutDefinition;

        protected override void CacheProperties(ref Dictionary<SoundProperty, Definition> soundProperties)
        {
            CacheProperties();
            soundProperties.Add(_pitchProperty, pitchPropertyDefinition);
            soundProperties.Add(_volumeProperty, volumePropertyDefinition);
            soundProperties.Add(_delayProperty, delayPropertyDefinition);
        }
        
        #region Private Static Methods
        private static void CacheProperties()
        {
            if (_propertiesCached) return;
            _pitchProperty = BuiltInData.Properties.GetSoundPropertyByType<Pitch>();
            _volumeProperty = BuiltInData.Properties.GetSoundPropertyByType<Volume>();
            _delayProperty = BuiltInData.Properties.GetSoundPropertyByType<Delay>();
            _propertiesCached = true;
        }
        #endregion
    }
}