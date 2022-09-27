using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HearXR.Audiobread
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TestSoundDefinition), true)]
    public class TestSoundDefinitionEditor : SoundDefinitionEditor {}
}