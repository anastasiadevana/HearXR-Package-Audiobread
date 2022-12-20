using UnityEngine;

namespace HearXR.Audiobread
{
    [CreateAssetMenu(fileName = "Clip", menuName = "Audiobread/Sound Generator/Clip")]
    public class AudiobreadClipDefinition : SoundDefinition
    {
        #region Editor Fields
        [SerializeField] private AudioClip _audioClip;
        #endregion
        
        #region Properties
        public AudioClip AudioClip
        {
            get => _audioClip;
            set => _audioClip = value;
        }
        #endregion

        #region ISoundDefinition Methods
        public override T CreateSound<T>(InitSoundFlags initSoundFlags = InitSoundFlags.None)
        {
            var sound = new AudiobreadClipWrapper();
            ((ISoundInternal<AudiobreadClipDefinition>) sound).Init(this, initSoundFlags);
            return sound as T;
        }
        #endregion
    }
}