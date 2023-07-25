using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HearXR.Audiobread
{
    [CreateAssetMenu(menuName = "Audiobread/Sound Module Definitions/Core Child Picker")]
    public class CoreChildPickerSoundModuleDefinition : SoundModuleDefinition, IChildPickerDefinition
    {
        [SerializeField] private ParentSoundPlaybackOrder _playbackOrder;
        [SerializeField] private bool _doNotRepeatLast = true;

        public bool DoNotRepeatLast => _doNotRepeatLast;
        public ParentSoundPlaybackOrder PlaybackOrder => _playbackOrder;
        
        [System.NonSerialized] internal int definitionSharedLastIndex;
    }
}