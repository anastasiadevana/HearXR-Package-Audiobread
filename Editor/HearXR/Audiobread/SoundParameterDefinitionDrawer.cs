using HearXR.Audiobread.SoundProperties;
using UnityEditor;
using UnityEngine;

namespace HearXR.Audiobread
{
    [CustomPropertyDrawer(typeof(SoundParameterDefinition), true)]
    public class SoundParameterDefinitionDrawer : PropertyDrawer
    {
        private Vector2 _hasMin = Vector2.zero;
        private Vector2 _hasMax = Vector2.zero;
        private Vector2 _min = Vector2.zero;
        private Vector2 _max = new Vector2(10, 1);
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var numLines = 5;
            return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * numLines + 14;
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var parameterProp = property.FindPropertyRelative("parameter");
            var soundPropertyProp = property.FindPropertyRelative("soundProperty");
            var curveProp = property.FindPropertyRelative("curve");
            var calculationMethodProp = property.FindPropertyRelative("calculationMethod");

            EditorGUI.BeginChangeCheck();
            
            position.height = EditorGUIUtility.singleLineHeight;
            
            parameterProp.objectReferenceValue = EditorGUI.ObjectField(position, new GUIContent("Parameter"), parameterProp.objectReferenceValue, typeof(Parameter),
                false);
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

                var parameter = (HearXR.Audiobread.Parameter) parameterProp.objectReferenceValue;
                var soundProperty = (SoundProperty) soundPropertyProp.objectReferenceValue;

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

                    // var curve = curveProp.animationCurveValue;
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
        }
    }
}