using UnityEditor;
using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    [CustomPropertyDrawer(typeof(TimeDurationDefinition))]
    public class TimeDurationDefinitionDrawer : FloatDefinitionDrawer
    {
        protected override (FloatSoundProperty, float) GetSoundPropertyAndDefaultValue()
        {
            return GetSoundPropertyAndDefaultValue<TimeDuration>();
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // TODO: Really don't need a tuple here! Can just get the property.
            var (soundProperty, defaultValue) = GetSoundPropertyAndDefaultValue();
            if (soundProperty == default)
            {
                Debug.LogError($"HEAR XR: Missing sound property definition drawer for {label.text}");
            }
            _soundProperty = soundProperty;

            var valueProp = property.FindPropertyRelative("value");
            var randomizeProp = property.FindPropertyRelative("randomize");
            var varianceProp = property.FindPropertyRelative("variance");
            var soundPropertyProp = property.FindPropertyRelative("soundProperty");

            // Make sure we save the sound property reference.
            if (soundPropertyProp.objectReferenceValue == null)
            {
                soundPropertyProp.objectReferenceValue = soundProperty;
                valueProp.floatValue = defaultValue;
            }

            // Cache some values.
            _rowX = position.x;
            _rowWidth = position.width;
            _controlX = position.x;
            _controlWidth = position.width;
            float startingY = position.y;

            // Draw header.
            position = DrawHeader2(position, _soundProperty.name);

            // Draw checkbox and "Randomize" label overlapping the color rectangle.
            float nextY = position.y;
            position.y = startingY;
            position.x = _controlX + (_controlWidth - _checkboxWidth - _horizontalSpacer);
            position.width = _checkboxWidth;

            _showRandomize = false;
            if (_soundProperty.Randomizable)
            {
                randomizeProp.boolValue = EditorGUI.Toggle(position, randomizeProp.boolValue);
                _showRandomize = randomizeProp.boolValue;
            }

            // Label
            position.x -= (_randomizeLabelWidth + _horizontalSpacer);
            position.width = _randomizeLabelWidth;
            position.y -= 1; // Slight tweaking to make sure it all lines up nicely.
            if (_soundProperty.Randomizable)
            {
                EditorGUI.LabelField(position, "Randomize");
            }
            position.y = nextY;
            
            
            
            // TODO: Add a checkbox for DEFAULT duration.


            // TODO: Take a look at this, and do something like that:
            //       https://github.com/roboryantron/Unite2017/blob/master/Assets/Code/Variables/Editor/FloatReferenceDrawer.cs
            // // Calculate rect for configuration button
            // Rect buttonRect = new Rect(position);
            // buttonRect.yMin += popupStyle.margin.top;
            // buttonRect.width = popupStyle.fixedWidth + popupStyle.margin.right;
            // position.xMin = buttonRect.xMax;
            // // Store old indent level and set it to 0, the PrefixLabel takes care of it
            // int indent = EditorGUI.indentLevel;
            // EditorGUI.indentLevel = 0;
            // int result = EditorGUI.Popup(buttonRect, useConstant ? 0 : 1, popupOptions, popupStyle);
            // useConstant = result == 0;

            // Draw the actual base value slider.
            // TODO: Handle if the sound property doesn't have one or both of the limits.
            // Leave a little room on the sides, so that the labels don't get cut off.
            position.x = _rowX + _baseValueSliderLeftOffset;
            position.width = _rowWidth - _baseValueSliderLeftOffset - _baseValueSliderRightOffset;
            position.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.Slider(position, valueProp, _soundProperty.MinLimit, _soundProperty.MaxLimit, "");
            // Debug.Log($"Create slider for {soundProperty} current value {valueProp.floatValue} min: {_soundProperty.MinLimit} max: {_soundProperty.MaxLimit}");

            // Display min / max values if randomization is enabled.
            if (_showRandomize)
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
                float clampedMin = Mathf.Clamp((valueProp.floatValue - varianceProp.floatValue), minLimit, maxLimit);
                float clampedMax = Mathf.Clamp((valueProp.floatValue + varianceProp.floatValue), minLimit, maxLimit);

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
                if (varianceProp.floatValue > 0.0f)
                {
                    ReadOnlyLabelStyle.alignment = TextAnchor.MiddleRight;
                    EditorGUI.LabelField(minRect, formattedMin, ReadOnlyLabelStyle);

                    ReadOnlyLabelStyle.alignment = TextAnchor.MiddleLeft;
                    EditorGUI.LabelField(maxRect, formattedMax, ReadOnlyLabelStyle);
                }

                position.y += 13;
                EditorGUIUtility.labelWidth = 60;
                float totalPropertyRange = Mathf.Abs(_soundProperty.MaxLimit - _soundProperty.MinLimit);
                EditorGUI.Slider(position, varianceProp, 0.0f, totalPropertyRange, "Variance");
            }
        }
    }
}