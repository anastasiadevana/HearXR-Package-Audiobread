using UnityEditor;
using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    [CustomPropertyDrawer(typeof(Definition), true)]
    public class DefinitionDrawer : PropertyDrawer
    {
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
        
        #region Protected Methods
        protected Rect DrawHeader2(Rect pos, string label)
        {
            // Draw background color for the header.
            pos.height = _header2RectHeight;
            EditorGUI.DrawRect(pos, _header2RectColor);

            // Draw the header label.
            pos.x += 6;
            EditorGUI.LabelField(pos, label, Header2Style);
            pos.x -= 6;

            pos.y += _header2RectHeight + (EditorGUIUtility.standardVerticalSpacing * 3);
            
            return pos;
        }
        #endregion
    }
}