using UnityEditor;
using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    [CustomPropertyDrawer(typeof(IntDefinition), true)]
    public class IntDefinitionDrawer : DefinitionDrawer
    {
        // TODO: Maybe turn this into something like this? https://docs.unity3d.com/ScriptReference/PopupWindow.html
        // /// <summary>
        // /// Options to display in the popup to select constant or variable.
        // /// </summary>
        // private readonly string[] popupOptions = { "Use Constant", "Use Variable" };
        // bool useConstant;
        //
        // /// <summary> Cached style to use to draw the popup button. </summary>
        // private GUIStyle popupStyle;
        
        private IntSoundProperty _soundProperty;

        private bool _showRandomize;

        private SerializedProperty _audioClipsProp;
        private SerializedProperty _volumeProp;
        private SerializedProperty _randomizeVolumeProp;
        private SerializedProperty _volumeVarianceProp;
        private SerializedProperty _pitchProp;
        private SerializedProperty _randomizePitchProp;
        private SerializedProperty _pitchVarianceProp;
        private SerializedProperty _loopingProp;
        private SerializedProperty _loopType;
        private SerializedProperty _timeBetweenClipsProp;
        private SerializedProperty _randomizeTimeBetweenClipsProp;
        private SerializedProperty _timeBetweenClipsVarianceProp;
        private SerializedProperty _spatializationTypeProp;

        private const float UNITY_SLIDER_NUMBER_WIDTH = 55.0f;

        private readonly float _labelWidth = 120.0f;
        private readonly float _smallNumberInputWidth = 50.0f;
        private readonly float _randomizeLabelWidth = 70.0f;
        private readonly float _horizontalSpacer = 5.0f;
        private readonly float _checkboxWidth = 16.0f;
        private readonly float _baseValueSliderLeftOffset = 20.0f; // used to be 26.0f;

        private readonly float _baseValueSliderRightOffset = 16.0f;
        private readonly Color _minMaxSliderOverlayColor = new Color(0.957f, 0.098f, 0.976f);

        private readonly float _header3RectHeight = 24.0f;
        private readonly Color _header3RectColor = Color.yellow;

        private float _rowX;
        private float _rowWidth;
        private float _controlX;
        private float _controlWidth;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int numLines = (_showRandomize) ? 4 : 2;
            return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * numLines + 14;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // if (popupStyle == null)
            // {
            //     popupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions"));
            //     popupStyle.imagePosition = ImagePosition.ImageOnly;
            // }
            
            // TODO: Really don't need a tuple here! Can just get the property.
            (IntSoundProperty soundProperty, int defaultValue) = GetSoundPropertyAndDefaultValue();
            if (soundProperty == default)
            {
                Debug.LogError($"HEAR XR: Missing sound property definition drawer for {label.text}");
            }
            _soundProperty = soundProperty;

            SerializedProperty valueProp = property.FindPropertyRelative("value");
            SerializedProperty randomizeProp = property.FindPropertyRelative("randomize");
            SerializedProperty varianceProp = property.FindPropertyRelative("variance");
            SerializedProperty soundPropertyProp = property.FindPropertyRelative("soundProperty");

            // Make sure we save the sound property reference.
            if (soundPropertyProp.objectReferenceValue == null)
            {
                soundPropertyProp.objectReferenceValue = soundProperty;
                valueProp.intValue = defaultValue;
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
            randomizeProp.boolValue = EditorGUI.Toggle(position, randomizeProp.boolValue);
            _showRandomize = randomizeProp.boolValue;

            // Label
            position.x -= (_randomizeLabelWidth + _horizontalSpacer);
            position.width = _randomizeLabelWidth;
            position.y -= 1; // Slight tweaking to make sure it all lines up nicely.
            EditorGUI.LabelField(position, "Randomize");
            position.y = nextY;
            
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
            //EditorGUI.Slider(position, valueProp, _soundProperty.MinLimit, _soundProperty.MaxLimit, "");
            //EditorGUI.IntSlider(position, valueProp, _soundProperty.MaxLimit, _soundProperty.MaxLimit, "");
            //Debug.Log($"current value {valueProp.intValue} min {_soundProperty.MinLimit} max {_soundProperty.MaxLimit}");
            EditorGUI.IntSlider(position, valueProp, _soundProperty.MinLimit, _soundProperty.MaxLimit, "");

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

                int minLimit = _soundProperty.MinLimit;
                int maxLimit = _soundProperty.MaxLimit;

                // Calculate and clamp min/max values.
                int clampedMin = Mathf.Clamp((valueProp.intValue - varianceProp.intValue), minLimit, maxLimit);
                int clampedMax = Mathf.Clamp((valueProp.intValue + varianceProp.intValue), minLimit, maxLimit);

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
                string formattedMin = clampedMin.ToString(); // $"{clampedMin:0.###}";
                string formattedMax = clampedMax.ToString(); // $"{clampedMax:0.###}";

                float inverseOffset = 4.0f;

                Rect minRect = new Rect(overlay.x - _smallNumberInputWidth - _horizontalSpacer,
                    controlY - inverseOffset, _smallNumberInputWidth, controlHeight);
                Rect maxRect = new Rect(overlay.x + overlay.width + _horizontalSpacer, controlY - inverseOffset,
                    _smallNumberInputWidth, controlHeight);

                // Align and draw the labels.
                if (varianceProp.intValue > 0)
                {
                    ReadOnlyLabelStyle.alignment = TextAnchor.MiddleRight;
                    EditorGUI.LabelField(minRect, formattedMin, ReadOnlyLabelStyle);

                    ReadOnlyLabelStyle.alignment = TextAnchor.MiddleLeft;
                    EditorGUI.LabelField(maxRect, formattedMax, ReadOnlyLabelStyle);
                }

                position.y += 13;
                EditorGUIUtility.labelWidth = 60;
                int totalPropertyRange = Mathf.Abs(_soundProperty.MaxLimit - _soundProperty.MinLimit);
                //EditorGUI.Slider(position, varianceProp, 0.0f, totalPropertyRange, "Variance");
                EditorGUI.IntSlider(position, varianceProp, 0, totalPropertyRange, "Variance");
            }
        }

        protected virtual (IntSoundProperty, int) GetSoundPropertyAndDefaultValue()
        {
            return default;
        }
    }
}