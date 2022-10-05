using System;
using System.Collections.Generic;
using System.Reflection;
using HearXR.Audiobread.SoundProperties;
using UnityEngine;

namespace HearXR.Audiobread
{
    // TODO: Add support for custom cutoff curve.
    public class LowPassSoundModuleDefinition : SoundModuleDefinition
    {
        #region Static Private Fields
        private static bool _propertiesCached;
        private static LowPassCutoffFrequency _lowPassCutoffFrequencyProperty;
        private static LowPassResonanceQ _lowPassResonanceQProperty;
        #endregion
        
        #region Static Properties
        public static LowPassCutoffFrequency LowPassCutoffFrequencyProperty
        {
            get
            {
                CacheProperties();
                return _lowPassCutoffFrequencyProperty;
            }
        }

        public static LowPassResonanceQ LowPassResonanceQProperty
        {
            get
            {
                CacheProperties();
                return _lowPassResonanceQProperty;
            }
        }
        #endregion
        
        public LowPassCutoffFrequencyDefinition lowPassCutoffFrequency;
        public LowPassResonanceQDefinition lowPassResonanceQ;

        protected override void CacheProperties(ref Dictionary<SoundProperty, Definition> soundProperties)
        {
            CacheProperties();
            soundProperties.Add(_lowPassCutoffFrequencyProperty, lowPassCutoffFrequency);
            soundProperties.Add(_lowPassResonanceQProperty, lowPassResonanceQ);
        }
        
        #region Private Static Methods
        private static void CacheProperties()
        {
            if (_propertiesCached) return;
            _lowPassCutoffFrequencyProperty = BuiltInData.Properties.GetSoundPropertyByType<LowPassCutoffFrequency>();
            _lowPassResonanceQProperty = BuiltInData.Properties.GetSoundPropertyByType<LowPassResonanceQ>();
            _propertiesCached = true;
        }
        #endregion
    }
}