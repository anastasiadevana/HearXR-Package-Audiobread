using System;
using System.Collections.Generic;
using HearXR.Audiobread.SoundProperties;
using UnityEngine;

// TODO: Delete this already.
namespace HearXR.Audiobread
{
    /// <summary>
    /// TestSoundDefinition with new property stuff.
    /// </summary>
    [CreateAssetMenu(fileName = "TestSoundDefinition", menuName = "Audiobread/Test Sound Definition")]
    public class TestSoundDefinition : SoundDefinition, ISoundGeneratorUnityAudioDefinition
    {
        #region Editor Fields
        [SerializeField] private AudioClip _audioClip;
        public AudioClip AudioClip => _audioClip;
        #endregion

        #region ISoundDefinition Methods
        public override T CreateSound<T>(InitSoundFlags initSoundFlags = InitSoundFlags.None)
        {
            var sound = new TestSound();
            ((ISoundInternal<TestSoundDefinition>) sound).Init(this, initSoundFlags);
            return sound as T;
        }
        #endregion
    }
}