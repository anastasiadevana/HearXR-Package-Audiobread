using System.Collections.Generic;
using HearXR.Audiobread.SoundProperties;
using UnityEngine;

namespace HearXR.Audiobread
{
    [CreateAssetMenu(menuName = "Audiobread/Sound Module Definitions/Echo")]
    public class EchoSoundModuleDefinition : SoundModuleDefinition
    {
        #region Static Private Fields
        private static bool _propertiesCached;
        private static EchoDelay _echoDelayProperty;
        private static EchoDecayRatio _echoDecayRatioProperty;
        private static EchoDryMix _echoDryMixProperty;
        private static EchoWetMix _echoWetMixProperty;
        #endregion
        
        #region Static Properties
        public static EchoDelay EchoDelayProperty
        {
            get
            {
                CacheProperties();
                return _echoDelayProperty;
            }
        }

        public static EchoDecayRatio EchoDecayRatioProperty
        {
            get
            {
                CacheProperties();
                return _echoDecayRatioProperty;
            }
        }

        public static EchoDryMix EchoDryMixProperty
        {
            get
            {
                CacheProperties();
                return _echoDryMixProperty;
            }
        }
        
        public static EchoWetMix EchoWetMixProperty
        {
            get
            {
                CacheProperties();
                return _echoWetMixProperty;
            }
        }
        #endregion
        
        public EchoDelayDefinition delay;
        public EchoDecayRatioDefinition decayRatio;
        public EchoDryMixDefinition dryMix;
        public EchoWetMixDefinition wetMix;

        protected override void CacheProperties(ref Dictionary<SoundProperty, Definition> soundProperties)
        {
            CacheProperties();
            soundProperties.Add(_echoDelayProperty, delay);
            soundProperties.Add(_echoDecayRatioProperty, decayRatio);
            soundProperties.Add(_echoDryMixProperty, dryMix);
            soundProperties.Add(_echoWetMixProperty, wetMix);
        }
        
        #region Private Static Methods
        private static void CacheProperties()
        {
            if (_propertiesCached) return;
            _echoDelayProperty = BuiltInData.Properties.GetSoundPropertyByType<EchoDelay>();
            _echoDecayRatioProperty = BuiltInData.Properties.GetSoundPropertyByType<EchoDecayRatio>();
            _echoDryMixProperty = BuiltInData.Properties.GetSoundPropertyByType<EchoDryMix>();
            _echoWetMixProperty = BuiltInData.Properties.GetSoundPropertyByType<EchoWetMix>();
            _propertiesCached = true;
        }
        #endregion
    }
}