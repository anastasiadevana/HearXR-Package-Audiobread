using UnityEngine;

namespace HearXR.Audiobread
{
    [CreateAssetMenu(fileName = "SimpleSampler", menuName = "Audiobread/Sound Generator/Simple Sampler")]
    public class SimpleSamplerDefinition : SoundDefinition
    {
        #region Editor Fields
        [SerializeField] private AudioClip _audioClip;
        #endregion
        
        #region Properties
        public AudioClip AudioClip => _audioClip;
        #endregion

        #region ISoundDefinition Methods
        public override T CreateSound<T>(InitSoundFlags initSoundFlags = InitSoundFlags.None)
        {
            var sound = new SimpleSamplerWrapper();
            ((ISoundInternal<SimpleSamplerDefinition>) sound).Init(this, initSoundFlags);
            return sound as T;
        }
        #endregion
    }
}