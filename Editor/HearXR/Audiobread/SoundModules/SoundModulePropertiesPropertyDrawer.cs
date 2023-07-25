using UnityEditor;
using UnityEngine;

namespace HearXR.Audiobread
{
    [CustomPropertyDrawer(typeof(SoundModuleDefinition))]
    public class SoundModulePropertiesPropertyDrawer : PropertyDrawer
    {
        private Editor _editor;
        private readonly float _buttonWidth = 78.0f;
        private readonly float _buttonLeftMargin = 2.0f;
        private bool _showProperty;
        private SoundModuleDefinition _soundModuleDefinition;
        private string _foldoutLabel = string.Empty;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var embeddedInSoundDefinition = (property.serializedObject.targetObject is SoundDefinition);
            
            _soundModuleDefinition = property.objectReferenceValue as SoundModuleDefinition;

            EditorGUI.BeginProperty(position, label, property);

            if (embeddedInSoundDefinition)
            {
                // There may or may not be a sound module definition assigned yet.
                if (_soundModuleDefinition == null)
                {
                    // Do nothing. This is a mistake and should be ignored.
                }

                // There is already a sound parameter definition assigned.
                else
                {
                    // Display the object selector field and a button to clone it.
                    EditorGUI.ObjectField(
                        new Rect(position.x, position.y, position.width - (_buttonWidth + _buttonLeftMargin),
                            EditorGUIUtility.singleLineHeight), property);
                    
                    if (GUI.Button(
                        new Rect(position.x + position.width - _buttonWidth, position.y, _buttonWidth,
                            EditorGUIUtility.singleLineHeight), "Clone"))
                    {
                        var newSoundModuleDefinition = AudiobreadEditorUtilities.CreateNewAudiobreadAsset(_soundModuleDefinition);
                        if (newSoundModuleDefinition != null)
                        {
                            property.objectReferenceValue = newSoundModuleDefinition;
                            property.serializedObject.ApplyModifiedProperties();
                        }
                        GUIUtility.ExitGUI();
                    }

                    if (GUI.changed)
                    {
                        property.serializedObject.ApplyModifiedProperties();
                    }

                    // If user unassigned the object, stop it right here.
                    if (property.objectReferenceValue == null)
                    {
                        EditorGUI.EndProperty();
                        GUIUtility.ExitGUI();
                    }
                    
                    
                    
                    if (_soundModuleDefinition != null)
                    {
                        _foldoutLabel = _soundModuleDefinition.soundModule.DisplayName;
                    }
                    else
                    {
                        _foldoutLabel = (_showProperty) ? "collapse" : "expand";   
                    }
                    // TODO: Make the foldout label bolder.
                    _showProperty = EditorGUILayout.Foldout(_showProperty, _foldoutLabel);
                    
                    if (_showProperty)
                    {
                        Editor.CreateCachedEditor(property.objectReferenceValue, typeof(SoundModulePropertiesEditor), ref _editor);
                        var editorPosition = position;
                        editorPosition.height = EditorGUIUtility.singleLineHeight;
                        editorPosition.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        ((SoundModulePropertiesEditor) _editor).SetPosition(editorPosition);
                        _editor.OnInspectorGUI();
                    
                    }
                }

                if (GUI.changed)
                {
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }

            EditorGUI.EndProperty();
        }
    }
}