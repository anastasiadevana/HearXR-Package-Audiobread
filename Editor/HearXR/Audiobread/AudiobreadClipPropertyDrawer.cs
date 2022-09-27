using UnityEditor;
using UnityEngine;

namespace HearXR.Audiobread
{
    [CustomPropertyDrawer(typeof(AudiobreadClipDefinition), true)]
    public class AudiobreadClipPropertyDrawer : PropertyDrawer
    {
        private Editor _editor;
        private bool _hasSubEditor;
 
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2;
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, true);
            _hasSubEditor = property.objectReferenceValue != null;
            
            if (_hasSubEditor)
            {
                Editor.CreateCachedEditor(property.objectReferenceValue, typeof(AudiobreadClipDefinitionEditor), ref _editor);
                Rect editorPosition = position;
                editorPosition.height = EditorGUIUtility.singleLineHeight;
                editorPosition.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                ((AudiobreadClipDefinitionEditor) _editor).SetPosition(editorPosition);
                _editor.OnInspectorGUI();
            }
        }
    }
}
