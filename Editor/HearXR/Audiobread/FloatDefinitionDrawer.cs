﻿using UnityEditor;
using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    [CustomPropertyDrawer(typeof(FloatDefinition), true)]
    public class FloatDefinitionDrawer : DefinitionDrawer
    {
        protected FloatSoundProperty _soundProperty;

        protected override bool TryInitSoundProperty(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!(property.serializedObject.targetObject is ISoundModuleDefinition))
            {
                Debug.LogError($"{property.serializedObject.targetObject} is not an ISoundModuleDefinition");
            }
            
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
                _valueProp.floatValue = defaultValue;
                _activeProp.boolValue = _soundProperty.ActiveByDefault;
            }

            _soundPropertyName = _soundProperty.name;
            _soundPropertyRandomizable = _soundProperty.Randomizable;
            
            return true;
        }

        protected override void DrawSoundPropertySlider(Rect position)
        {
            EditorGUI.Slider(position, _valueProp, _soundProperty.MinLimit, _soundProperty.MaxLimit, "");
        }

        protected override void DrawRandomizationRange(Rect position)
        {
            // Real quick do the magic number offsets.
            float offsetRowWidth = _rowWidth - _baseValueSliderLeftOffset - _baseValueSliderRightOffset;
            float offsetRowX = _rowX + _baseValueSliderLeftOffset;

            position.y += position.height + EditorGUIUtility.standardVerticalSpacing;

            float controlY = position.y;
            float controlHeight = position.height;

            // Calculations for drawing the min-max range over the slider.
            float sliderWidth = offsetRowWidth - UNITY_SLIDER_NUMBER_WIDTH;
            
            float minLimit = _soundProperty.MinLimit;
            float maxLimit = _soundProperty.MaxLimit;

            // Calculate and clamp min/max values.
            float clampedMin = Mathf.Clamp((_valueProp.floatValue - _varianceProp.floatValue), minLimit, maxLimit);
            float clampedMax = Mathf.Clamp((_valueProp.floatValue + _varianceProp.floatValue), minLimit, maxLimit);

            // Normalize the min/max values to 0-1 range.
            float normalizedMin = Mathf.InverseLerp(minLimit, maxLimit, clampedMin);
            float normalizedMax = Mathf.InverseLerp(minLimit, maxLimit, clampedMax);

            // Draw the min-max slider overlay (shift it upwards to overlay with the slider).
            Rect overlay = position;
            overlay.y -= 19; // Used to be 20
            overlay.x = offsetRowX + sliderWidth * normalizedMin;
            overlay.width = sliderWidth * (normalizedMax - normalizedMin);
            overlay.height = 16.0f; // This wasn't here
            EditorGUI.DrawRect(overlay, _minMaxSliderOverlayColor);

            // Display the min and max labels.
            string formattedMin = $"{clampedMin:0.###}";
            string formattedMax = $"{clampedMax:0.###}";

            float inverseOffset = 4.0f;

            Rect minRect = new Rect(overlay.x - _smallNumberInputWidth - _horizontalSpacer,
                controlY - inverseOffset, _smallNumberInputWidth, controlHeight);
            Rect maxRect = new Rect(overlay.x + overlay.width + _horizontalSpacer, controlY - inverseOffset,
                _smallNumberInputWidth, controlHeight);

            // Align and draw the labels.
            if (_varianceProp.floatValue > 0.0f)
            {
                ReadOnlyLabelStyle.alignment = TextAnchor.MiddleRight;
                EditorGUI.LabelField(minRect, formattedMin, ReadOnlyLabelStyle);

                ReadOnlyLabelStyle.alignment = TextAnchor.MiddleLeft;
                EditorGUI.LabelField(maxRect, formattedMax, ReadOnlyLabelStyle);
            }
            
            position.y += 13;
            EditorGUIUtility.labelWidth = 60;
            float totalPropertyRange = Mathf.Abs(_soundProperty.MaxLimit - _soundProperty.MinLimit);
            EditorGUI.Slider(position, _varianceProp, 0.0f, totalPropertyRange, "Variance");
        }
    }
}