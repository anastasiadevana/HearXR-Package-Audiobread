using UnityEditor;
using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    [CustomPropertyDrawer(typeof(BoolDefinition), true)]
    public class BoolDefinitionDrawer : DefinitionDrawer
    {
        private BoolSoundProperty _soundProperty;
        private BoolDefinition _boolDefinition;

        #region GUI Methods
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var numLines = 1;
            if (_active)
            {
                numLines = 2;
            }
            return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * numLines + 14;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _valueProp = property.FindPropertyRelative("value");
            _randomizeProp = property.FindPropertyRelative("randomize");
            _varianceProp = property.FindPropertyRelative("variance");
            _soundPropertyProp = property.FindPropertyRelative("soundProperty");
            _activeProp = property.FindPropertyRelative("active");

            if (!TryInitSoundProperty(position, property, label))
            {
                Debug.Log("Unable to init sound property!");
                return;
            }
            
            // Cache some values.
            _rowX = position.x;
            _rowWidth = position.width;
            _controlX = position.x;
            _controlWidth = position.width;
            var startingY = position.y;

            // Draw header.
            position = DrawHeader2(position, _soundPropertyName);

            
            // Draw the activator checkbox.
            // TODO: Move this before the header?
            var nextY = position.y;
            position.y = startingY;
            position.x = _controlX + 6; // TODO: No magic numbers
            position.width = _checkboxWidth;
            
            _activeProp.boolValue = EditorGUI.Toggle(position, _activeProp.boolValue);
            _active = _activeProp.boolValue;

            if (_active)
            {
                 // Draw checkbox and "Randomize" label overlapping the color rectangle.
                position.x = _controlX + (_controlWidth - _checkboxWidth - _horizontalSpacer);
                position.width = _checkboxWidth;

                _showRandomize = false;
                if (_soundPropertyRandomizable)
                {
                    _randomizeProp.boolValue = EditorGUI.Toggle(position, _randomizeProp.boolValue);
                    _showRandomize = _randomizeProp.boolValue;
                }

                // Label
                position.x -= (_randomizeLabelWidth + _horizontalSpacer);
                position.width = _randomizeLabelWidth;
                position.y -= 1; // Slight tweaking to make sure it all lines up nicely.
                if (_soundPropertyRandomizable)
                {
                    EditorGUI.LabelField(position, "Randomize");
                }
                position.y = nextY;
                
                position.x = _rowX + _baseValueSliderLeftOffset;
                position.width = _rowWidth - _baseValueSliderLeftOffset - _baseValueSliderRightOffset;
                position.height = EditorGUIUtility.singleLineHeight;
                
                EditorGUI.BeginChangeCheck();
                
                var boolValue = EditorGUI.Toggle(position, _valueProp.boolValue);
                position.x += 20;
                EditorGUI.LabelField(position, _soundPropertyName);
                
                if (EditorGUI.EndChangeCheck())
                {
                    _valueProp.boolValue = boolValue;
                }
            }
        }
        #endregion

        #region Virtual Methods
        protected override bool TryInitSoundProperty(Rect position, SerializedProperty property, GUIContent label)
        {
            var soundModuleDefinition = (ISoundModuleDefinition) property.serializedObject.targetObject;
            if (soundModuleDefinition == null)
            {
                Debug.LogError($"Unable to cast sound module definition {property.serializedObject.targetObject}");
                return false;
            }

            var found = soundModuleDefinition.TryGetSoundProperty(fieldInfo, out _soundProperty);

            if (!found)
            {
                Debug.LogError($"HEAR XR: Missing sound property definition drawer for {label.text}");
                return false;
            }

            var defaultValue = _soundProperty.DefaultValue;

            // Make sure we save the sound property reference.
            if (_soundPropertyProp.objectReferenceValue == null)
            {
                _soundPropertyProp.objectReferenceValue = _soundProperty;

                // Since this is the first time we're editing this definition, set default values.
                _valueProp.intValue = defaultValue;
                _activeProp.boolValue = _soundProperty.ActiveByDefault;
            }

            _soundPropertyName = _soundProperty.name;
            _soundPropertyRandomizable = _soundProperty.Randomizable;

            return true;
        }
        #endregion
    }
}