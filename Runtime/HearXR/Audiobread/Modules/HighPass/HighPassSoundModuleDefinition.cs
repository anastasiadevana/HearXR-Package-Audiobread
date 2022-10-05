using System;
using System.Collections.Generic;
using System.Reflection;
using HearXR.Audiobread.SoundProperties;
using UnityEngine;

namespace HearXR.Audiobread
{
    public class HighPassSoundModuleDefinition : SoundModuleDefinition
    {
        public HighPassCutoffFrequencyDefinition highPassCutoffFrequency;
        public HighPassResonanceQDefinition highPassResonanceQ;
        
        protected override void CacheProperties(ref Dictionary<SoundProperty, Definition> soundProperties)
        {
            soundProperties.Add(BuiltInData.Properties.GetSoundPropertyByType<HighPassCutoffFrequency>(), highPassCutoffFrequency);
            soundProperties.Add(BuiltInData.Properties.GetSoundPropertyByType<HighPassResonanceQ>(), highPassResonanceQ);
        }
    }
}