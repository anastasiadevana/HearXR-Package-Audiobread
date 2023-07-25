using System.Collections.Generic;
using HearXR.Audiobread.SoundProperties;
using UnityEngine;

namespace HearXR.Audiobread
{
    [CreateAssetMenu(menuName = "Audiobread/Sound Module Definitions/MSA")]
    public class MSASoundModuleDefinition : SoundModuleDefinition
    {
        protected override void CacheProperties(ref Dictionary<SoundProperty, Definition> soundProperties)
        {
            // TODO:
        }
    }
}
