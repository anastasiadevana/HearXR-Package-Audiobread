using UnityEditor;
using UnityEngine;

namespace HearXR.Audiobread
{
    [CustomEditor(typeof(SoundModuleDefinition))]
    public class SoundModulePropertiesEditor : Editor
    {
        private Rect _position;
        private bool _positionSet;
        private SoundModuleDefinition _soundModuleDefinition;
        
        // TODO: Move styles to a central place.
        protected GUIStyle Header2Style
        {
            get
            {
                if (_header2Style == null)
                {
                    _header2Style = new GUIStyle(EditorStyles.label);
                    _header2Style.fontStyle = FontStyle.Bold;
                    _header2Style.fontSize = 13;
                }

                return _header2Style;
            }
        }
        private GUIStyle _header2Style;
        
        public void SetPosition(Rect position)
        {
            _position = position;
            _positionSet = true;
            _soundModuleDefinition = (SoundModuleDefinition) target;
        }
        
        public override void OnInspectorGUI()
        {
            if (_positionSet)
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField(_soundModuleDefinition.soundModule.DisplayName, Header2Style); 
                EditorGUILayout.EndVertical();
            }
            
            DrawDefaultInspector();
        }
    }
}