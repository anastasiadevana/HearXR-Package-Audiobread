using System.Collections.Generic;
using HearXR.Audiobread.SoundProperties;

namespace HearXR.Audiobread
{
    public class LowPassSoundModuleDefinition : SoundModuleDefinition
    {
        public LowPassCutoffFrequencyDefinition lowPassCutoffFrequency;
        public LowPassResonanceQDefinition lowPassResonanceQ;
        
        protected override void CacheProperties(ref Dictionary<SoundProperty, Definition> soundProperties)
        {
            soundProperties.Add(BuiltInData.Properties.GetSoundPropertyByType<LowPassCutoffFrequency>(), lowPassCutoffFrequency);
            soundProperties.Add(BuiltInData.Properties.GetSoundPropertyByType<LowPassResonanceQ>(), lowPassResonanceQ);
        }
    }
}