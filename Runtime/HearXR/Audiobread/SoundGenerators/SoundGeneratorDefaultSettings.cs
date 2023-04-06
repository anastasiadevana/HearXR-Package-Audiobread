using UnityEngine;
using UnityEngine.Audio;

namespace HearXR.Audiobread
{
    [ExecuteAlways]
    [CreateAssetMenu(fileName = "SoundGeneratorDefaultSettings", menuName = "Audiobread/Sound Generator Default Settings")]
    public class SoundGeneratorDefaultSettings : ScriptableObject
    {
        #region Public Fields
        public float volume = 1.0f;
        public float pitch = 1.0f;

        public bool loop = false;
        public bool mute = false;
        public bool playOnAwake = false;
            
        public bool spatialize = false;
        public bool spatializePostEffects = false;
        [Range (0.0f, 1.0f)] public float spatialBlend = 0.0f;
        
        public bool bypassReverbZones = false;
        public bool bypassEffects = false;
        public bool bypassListenerEffects = false;

        public AudioMixerGroup audioMixerGroup = default;
        #endregion
    }
}
