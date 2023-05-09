using System.Collections.Generic;
using System.IO;
using HearXR.Audiobread.SoundProperties;
using UnityEditor;
using UnityEngine;

namespace HearXR.Audiobread
{
    [CustomPropertyDrawer(typeof(SoundParameterDefinition), true)]
    public class SoundParameterDefinitionDrawer : PropertyDrawer
    {
        #region Private Fields
        private SoundParameterDefinition _soundParameterDefinition;
        private SerializedObject _serializedObject;
        private Vector2 _hasMin = Vector2.zero;
        private Vector2 _hasMax = Vector2.zero;
        private Vector2 _min = Vector2.zero;
        private Vector2 _max = new Vector2(10, 1);
        private readonly float _buttonWidth = 78.0f;
        private readonly float _buttonLeftMargin = 2.0f;
        #endregion

        #region GUI
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var numLines = 6;
            return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * numLines + 14;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // There is no sound parameter definition assigned yet.
            if (property.objectReferenceValue == null)
            {
                // Display the object selector field and a button to create a new one.
                EditorGUI.ObjectField(
                    new Rect(position.x, position.y, position.width - (_buttonWidth + _buttonLeftMargin),
                        EditorGUIUtility.singleLineHeight), property);
                
                if (GUI.Button(
                    new Rect(position.x + position.width - _buttonWidth, position.y, _buttonWidth,
                        EditorGUIUtility.singleLineHeight), "Create"))
                {
                    var newSoundParameterDefinition = AudiobreadEditorUtilities.CreateNewAudiobreadAsset<SoundParameterDefinition>();
                    if (newSoundParameterDefinition != null)
                    {
                        property.objectReferenceValue = newSoundParameterDefinition;
                        property.serializedObject.ApplyModifiedProperties();
                    }
                    GUIUtility.ExitGUI();
                }
            }

            // There is already a sound parameter definition assigned.
            else
            {
                _soundParameterDefinition = property.objectReferenceValue as SoundParameterDefinition;

                // Display the object selector field and a button to clone it.
                EditorGUI.ObjectField(
                    new Rect(position.x, position.y, position.width - (_buttonWidth + _buttonLeftMargin),
                        EditorGUIUtility.singleLineHeight), property);
                
                if (GUI.Button(
                    new Rect(position.x + position.width - _buttonWidth, position.y, _buttonWidth,
                        EditorGUIUtility.singleLineHeight), "Clone"))
                {
                    var newSoundParameterDefinition = AudiobreadEditorUtilities.CreateNewAudiobreadAsset<SoundParameterDefinition>(property.objectReferenceValue as SoundParameterDefinition);
                    if (newSoundParameterDefinition != null)
                    {
                        property.objectReferenceValue = newSoundParameterDefinition;
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
                
                // Display the actual editor now.
                _serializedObject = new SerializedObject(_soundParameterDefinition);
                var parameterProp = _serializedObject.FindProperty("parameter");
                var soundPropertyProp = _serializedObject.FindProperty("soundProperty");
                var curveProp = _serializedObject.FindProperty("curve");
                var calculationMethodProp = _serializedObject.FindProperty("calculationMethod");

                EditorGUI.BeginChangeCheck();

                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                position.height = EditorGUIUtility.singleLineHeight;

                parameterProp.objectReferenceValue = EditorGUI.ObjectField(position, new GUIContent("Parameter"), parameterProp.objectReferenceValue, typeof(Parameter), false);
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                soundPropertyProp.objectReferenceValue = EditorGUI.ObjectField(position, new GUIContent("Sound Property"), soundPropertyProp.objectReferenceValue, typeof(SoundProperty),
                    false);
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                var selected = (CalculationMethod) calculationMethodProp.enumValueIndex;
                selected = (CalculationMethod) EditorGUI.EnumPopup(position, new GUIContent("Calculation Method"), selected);
                calculationMethodProp.enumValueIndex = (int) selected;
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                if (EditorGUI.EndChangeCheck())
                {
                    // Limit the curve by the min-max of the parameter and sound property.
                    // X is the parameter, Y is the sound property value.
                    var parameter = (Parameter) parameterProp.objectReferenceValue;
                    _soundParameterDefinition.parameter = parameter;

                    var soundProperty = (SoundProperty) soundPropertyProp.objectReferenceValue;
                    _soundParameterDefinition.soundProperty = soundProperty;

                    _soundParameterDefinition.calculationMethod = selected;

                    _hasMin.x = 1;
                    _hasMax.x = 1;

                    _min.x = parameter.minValue;
                    _max.x = parameter.maxValue;

                    if (parameter != null && soundProperty != null)
                    {
                        if (soundProperty.HasMinLimit)
                        {
                            _hasMin.y = 1;
                            if (soundProperty is DoubleSoundProperty doubleSoundProperty)
                            {
                                _min.y = (float) doubleSoundProperty.MinLimit;
                            }
                            else if (soundProperty is FloatSoundProperty floatSoundProperty)
                            {
                                _min.y = (float) floatSoundProperty.MinLimit;
                            }
                            else if (soundProperty is IntSoundProperty intSoundProperty)
                            {
                                _min.y = (float) intSoundProperty.MinLimit;
                            }
                            else if (soundProperty is EnumSoundProperty enumSoundProperty)
                            {
                                _min.y = (float) enumSoundProperty.MinLimit;
                            }
                        }

                        if (soundProperty.HasMaxLimit)
                        {
                            _hasMax.y = 1;
                            if (soundProperty is DoubleSoundProperty doubleSoundProperty)
                            {
                                _max.y = (float) doubleSoundProperty.MaxLimit;
                            }
                            else if (soundProperty is FloatSoundProperty floatSoundProperty)
                            {
                                _max.y = (float) floatSoundProperty.MaxLimit;
                            }
                            else if (soundProperty is IntSoundProperty intSoundProperty)
                            {
                                _max.y = (float) intSoundProperty.MaxLimit;
                            }
                            else if (soundProperty is EnumSoundProperty enumSoundProperty)
                            {
                                _max.y = (float) enumSoundProperty.MaxLimit;
                            }
                        }
                        
                        // If the curve was never created before, create a new one.
                        if (curveProp.animationCurveValue.length == 0)
                        {
                            curveProp.animationCurveValue = new AnimationCurve(
                                new Keyframe(_min.x, _min.y),
                                new Keyframe(_max.x, _max.y));
                        }
                    }
                }

                position.height = (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2;
                curveProp.animationCurveValue = EditorGUI.CurveField(position, "", curveProp.animationCurveValue);
                _soundParameterDefinition.curve = curveProp.animationCurveValue;
            }

            if (GUI.changed)
            {
                property.serializedObject.ApplyModifiedProperties();
            }
            
            EditorGUI.EndProperty();
        }
        #endregion
    }
}