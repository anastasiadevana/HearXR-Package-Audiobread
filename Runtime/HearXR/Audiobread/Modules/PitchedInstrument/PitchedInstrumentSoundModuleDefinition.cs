using System.Collections.Generic;
using HearXR.Audiobread.SoundProperties;

namespace HearXR.Audiobread
{
    public class PitchedInstrumentSoundModuleDefinition : SoundModuleDefinition
    {
        #region Static Private Fields
        private static bool _propertiesCached;
        private static NoteNumber _noteNumberProperty;
        #endregion
        
        #region Static Properties
        public static NoteNumber NoteNumberProperty
        {
            get
            {
                CacheProperties();
                return _noteNumberProperty;
            }
        }
        #endregion
        
        public NoteNumberDefinition noteNumber;

        protected override void CacheProperties(ref Dictionary<SoundProperty, Definition> soundProperties)
        {
            CacheProperties();
            soundProperties.Add(_noteNumberProperty, noteNumber);
        }
        
        #region Private Static Methods
        private static void CacheProperties()
        {
            if (_propertiesCached) return;
            _noteNumberProperty = BuiltInData.Properties.GetSoundPropertyByType<NoteNumber>();
            _propertiesCached = true;
        }
        #endregion
    }
}