using System.Collections.Generic;
using UnityEngine;

namespace HearXR.Audiobread
{
    public class CoreChildPickerSoundModuleDefinition : SoundModuleDefinition, IChildPickerDefinition
    {
        [SerializeField] private ParentSoundPlaybackOrder _playbackOrder;
        [SerializeField] private bool _doNotRepeatLast = true;

        public bool DoNotRepeatLast => _doNotRepeatLast;
        public ParentSoundPlaybackOrder PlaybackOrder => _playbackOrder;
        
        [System.NonSerialized] internal int definitionSharedLastIndex;
    }
}