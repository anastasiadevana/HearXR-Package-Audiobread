using UnityEditor;
using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    [CustomPropertyDrawer(typeof(Definition), true)]
    public class DefinitionDrawer : PropertyDrawer
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
        
        #region Properties
        protected GUIStyle ReadOnlyLabelStyle
        {
            get
            {
                if (_readOnlyLabelStyle == null)
                {
                    _readOnlyLabelStyle = new GUIStyle(EditorStyles.label);
                    _readOnlyLabelStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                }

                return _readOnlyLabelStyle;
            }
        }
        
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
        
        protected GUIStyle Header3Style
        {
            get
            {
                if (_header3Style == null)
                {
                    _header3Style = new GUIStyle(UnityEditor.EditorStyles.label);
                    _header3Style.fontStyle = FontStyle.Bold;
                    _header3Style.normal.textColor = new Color(0.957f, 0.098f, 0.976f);
                    _header3Style.fontSize = 11;
                }

                return _header3Style;
            }
        }
        
        protected GUIStyle HelpStyle
        {
            get
            {
                if (_helpStyle == null)
                {
                    _helpStyle = new GUIStyle(EditorStyles.label);
                    _helpStyle.normal.textColor = new Color(0.529f, 0.776f, 0.969f);
                }

                return _helpStyle;
            }
        }
        
        protected GUIStyle RandomizedStyle
        {
            get
            {
                if (_randomizedStyle == null)
                {
                    _randomizedStyle = new GUIStyle(EditorStyles.numberField);
                    _randomizedStyle.normal.textColor = Color.magenta;
                }

                return _randomizedStyle;
            }
        }
        #endregion

        #region Private Fields
        private GUIStyle _readOnlyLabelStyle;
        
        private GUIStyle _header2Style;
        private readonly float _header2RectHeight = 26.0f;
        private readonly Color _header2RectColor = new Color(0.016f, 0.02f, 0.137f);
        
        private GUIStyle _header3Style;
        
        private GUIStyle _helpStyle;
        
        private GUIStyle _randomizedStyle;
        #endregion
        
        #region Protected Fields
        protected SerializedProperty _valueProp;
        protected SerializedProperty _randomizeProp;
        protected SerializedProperty _varianceProp;
        protected SerializedProperty _soundPropertyProp;
        protected SerializedProperty _activeProp;
        
        protected bool _showRandomize;
        protected bool _active;
        
        protected readonly float _smallNumberInputWidth = 50.0f;
        protected readonly float _randomizeLabelWidth = 70.0f;
        protected readonly float _horizontalSpacer = 5.0f;
        protected readonly float _checkboxWidth = 16.0f;
        protected readonly float _baseValueSliderLeftOffset = 20.0f;
        
        protected readonly float _baseValueSliderRightOffset = 16.0f;
        protected readonly Color _minMaxSliderOverlayColor = new Color(0.957f, 0.098f, 0.976f);

        private readonly float _header3RectHeight = 24.0f;
        private readonly Color _header3RectColor = Color.yellow;
        
        protected float _rowX;
        protected float _rowWidth;
        protected float _controlX;
        protected float _controlWidth;
        
        protected const float UNITY_SLIDER_NUMBER_WIDTH = 55.0f;

        protected string _soundPropertyName;
        protected bool _soundPropertyRandomizable;
        #endregion
        
        #region GUI Methods
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var numLines = 1;
            if (_active)
            {
                numLines = (_showRandomize) ? 4 : 2; 
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
            
            if (!TryInitSoundProperty(position, property, label)) return;
            
            // Cache some values.
            _rowX = position.x;
            _rowWidth = position.width;
            _controlX = position.x;
            _controlWidth = position.width;
            float startingY = position.y;

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

                DrawSoundPropertySlider(position);

                // Display min / max values if randomization is enabled.
                if (_showRandomize)
                {
                    DrawRandomizationRange(position);
                }   
            }
        }
        #endregion

        #region Protected Methods
        protected Rect DrawHeader2(Rect pos, string label)
        {
            // Draw background color for the header.
            pos.height = _header2RectHeight;
            EditorGUI.DrawRect(pos, _header2RectColor);

            // Draw the header label.
            pos.x += 10 + 16.0f; // TODO: No magic numbers (checkbox width)
            EditorGUI.LabelField(pos, label, Header2Style);
            pos.x -= 10 - 16.0f; // TODO: No magic numbers (checkbox width)

            pos.y += _header2RectHeight + (EditorGUIUtility.standardVerticalSpacing * 3);
            
            return pos;
        }
        #endregion
        
        #region Virtual Methods
        protected virtual bool TryInitSoundProperty(Rect position, SerializedProperty property, GUIContent label)
        {
            return false;
        }
        
        protected virtual void DrawSoundPropertySlider(Rect position) {}
        
        protected virtual void DrawRandomizationRange(Rect position) {}
        #endregion
    }
}