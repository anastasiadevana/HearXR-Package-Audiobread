using System.Collections.Generic;
using HearXR.Audiobread.SoundProperties;
using UnityEngine;

namespace HearXR.Audiobread
{
    [CreateAssetMenu(fileName = "Tone Generator", menuName = "Audiobread/Sound Generator/Tone Generator")]
    public class ToneGeneratorDefinition : SoundDefinition
    {
        [SerializeField] private float _frequency = 220.0f; // TODO: Make this a sound property.
        
        public PitchDefinition pitchDefinition;
        
        public DelayDefinition delayDefinition;

        public float Frequency => _frequency;
        
        public override T CreateSound<T>(InitSoundFlags initSoundFlags = InitSoundFlags.None)
        {
            var sound = new ToneGeneratorWrapper();
            ((ISoundInternal<ToneGeneratorDefinition>) sound).Init(this, initSoundFlags);
            return sound as T;
        }
    }
}