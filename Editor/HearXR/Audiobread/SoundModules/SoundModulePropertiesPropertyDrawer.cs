using UnityEditor;
using UnityEngine;

namespace HearXR.Audiobread
{
    [CustomPropertyDrawer(typeof(SoundModuleDefinition))]
    public class SoundModulePropertiesPropertyDrawer : PropertyDrawer
    {
        private Editor _editor;
        private bool _hasSubEditor;
 
        // public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        // {
        //     return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 10;
        // }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, true);
            _hasSubEditor = property.objectReferenceValue != null;
            
            if (_hasSubEditor)
            {
                Editor.CreateCachedEditor(property.objectReferenceValue, typeof(SoundModulePropertiesEditor), ref _editor);
                Rect editorPosition = position;
                editorPosition.height = EditorGUIUtility.singleLineHeight;
                editorPosition.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                ((SoundModulePropertiesEditor) _editor).SetPosition(editorPosition);
                _editor.OnInspectorGUI();
            }
        }
    }
}