using UnityEngine;

namespace HearXR.Audiobread
{
    [CreateAssetMenu(fileName = "Tone Generator", menuName = "Audiobread/Sound Generator/Tone Generator")]
    public class ToneGeneratorDefinition : SoundDefinition, IToneGeneratorSoundDefinition, ISoundGeneratorUnityAudioDefinition
    {
        public override T CreateSound<T>(InitSoundFlags initSoundFlags = InitSoundFlags.None)
        {
            var sound = new ToneGeneratorWrapper();
            ((ISoundInternal<ToneGeneratorDefinition>) sound).Init(this, initSoundFlags);
            return sound as T;
        }
    }
}