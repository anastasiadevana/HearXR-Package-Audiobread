using System.Collections.Generic;
using HearXR.Audiobread.SoundProperties;
using UnityEngine;

namespace HearXR.Audiobread
{
    [CreateAssetMenu(fileName = "Clip", menuName = "Audiobread/Sound Generator/Clip")]
    public class AudiobreadClipDefinition : SoundDefinition
    {
        public int order;
        
        [SerializeField] private AudioClip _audioClip;
        
        public PitchDefinition pitchDefinition;
        
        public DelayDefinition delayDefinition;
        
        public OffsetDefinition offsetDefinition;

        [Range(0.0f, 1.0f)] public float weight = 0.5f;
        
        public AudioClip AudioClip => _audioClip;
        
        public override T CreateSound<T>(InitSoundFlags initSoundFlags = InitSoundFlags.None)
        {
            var sound = new AudiobreadClipWrapper();
            ((ISoundInternal<AudiobreadClipDefinition>) sound).Init(this, initSoundFlags);
            return sound as T;
        }
    }
}