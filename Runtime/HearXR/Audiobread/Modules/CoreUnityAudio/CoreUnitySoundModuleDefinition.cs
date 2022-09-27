using System.Collections.Generic;
using HearXR.Audiobread.SoundProperties;
using UnityEngine;

namespace HearXR.Audiobread.Core
{
    public class CoreUnitySoundModuleDefinition : SoundModuleDefinition
    {
        public PitchDefinition pitchPropertyDefinition;
        public VolumeDefinition volumePropertyDefinition;
        
        [Header("Fade In"), FadeValues(typeof(Volume), Fade.Direction.In)] public Fade fadeInDefinition;
        [Header("Fade Out"), FadeValues(typeof(Volume), Fade.Direction.Out)] public Fade fadeOutDefinition;

        internal Pitch PitchProperty
        {
            get
            {
                CacheProperties();
                return _pitchProperty;
            }
        }

        internal Volume VolumeProperty
        {
            get
            {
                CacheProperties();
                return _volumeProperty;
            }
        }

        // TODO: Turn these into static values. They should be cached in the XYZ_SoundModule, not in the definition.
        private bool _propertiesCached;
        private Pitch _pitchProperty;
        private Volume _volumeProperty;

        protected override void CacheProperties(ref Dictionary<SoundProperty, Definition> soundProperties)
        {
            CacheProperties();
            soundProperties.Add(_pitchProperty, pitchPropertyDefinition);
            soundProperties.Add(_volumeProperty, volumePropertyDefinition);
        }

        private void CacheProperties()
        {
            if (_propertiesCached) return;
            _pitchProperty = BuiltInData.Properties.GetSoundPropertyByType<Pitch>();
            _volumeProperty = BuiltInData.Properties.GetSoundPropertyByType<Volume>();
            _propertiesCached = true;
        }
    }
}