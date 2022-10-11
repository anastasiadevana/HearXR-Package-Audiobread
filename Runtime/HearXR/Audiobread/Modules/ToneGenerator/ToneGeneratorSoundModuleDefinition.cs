using System.Collections.Generic;
using HearXR.Audiobread.SoundProperties;

namespace HearXR.Audiobread
{
    public class ToneGeneratorSoundModuleDefinition : SoundModuleDefinition
    {
        #region Static Private Fields
        private static bool _propertiesCached;
        private static Frequency _frequencyProperty;
        private static WaveShape _waveShapeProperty;
        #endregion
        
        #region Static Properties
        public static Frequency FrequencyProperty
        {
            get
            {
                CacheProperties();
                return _frequencyProperty;
            }
        }

        public static WaveShape WaveShapeProperty
        {
            get
            {
                CacheProperties();
                return _waveShapeProperty;
            }
        }
        #endregion
        
        public FrequencyDefinition frequency;
        public WaveShapeDefinition waveShape;

        protected override void CacheProperties(ref Dictionary<SoundProperty, Definition> soundProperties)
        {
            CacheProperties();
            soundProperties.Add(_frequencyProperty, frequency);
            soundProperties.Add(_waveShapeProperty, waveShape);
        }
        
        #region Private Static Methods
        private static void CacheProperties()
        {
            if (_propertiesCached) return;
            _frequencyProperty = BuiltInData.Properties.GetSoundPropertyByType<Frequency>();
            _waveShapeProperty = BuiltInData.Properties.GetSoundPropertyByType<WaveShape>();
            _propertiesCached = true;
        }
        #endregion
    }
}