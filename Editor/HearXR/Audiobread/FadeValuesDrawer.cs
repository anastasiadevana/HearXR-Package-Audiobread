using UnityEditor;
using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    [CustomPropertyDrawer(typeof(FadeValuesAttribute))]
    public class FadeValuesDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // TODO: Validate property type. Must be FloatSoundPropertyDefinition.
            FadeValuesAttribute attributeSettings = (FadeValuesAttribute) attribute;

            FloatSoundProperty soundProperty = (FloatSoundProperty) BuiltInData.Instance.properties.GetSoundPropertyByType(attributeSettings.propertyType);
            
            Fade.Direction fadeDirection = attributeSettings.fadeDirection;
            
            SerializedProperty soundPropertyProp = property.FindPropertyRelative("soundProperty");
            SerializedProperty durationProp = property.FindPropertyRelative("duration");
            SerializedProperty directionProp = property.FindPropertyRelative("direction");
    
            // TODO: Check soundProperty for null.
            if (soundPropertyProp.objectReferenceValue == null)
            {
                soundPropertyProp.objectReferenceValue = soundProperty;
            }
    
            directionProp.enumValueIndex = (int) fadeDirection;
    
            position.height = EditorGUIUtility.singleLineHeight;
            position.y += EditorGUIUtility.standardVerticalSpacing;

            //EditorGUI.PropertyField(position, soundPropertyProp, true);
            EditorGUI.PropertyField(position, durationProp, true);
            position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
        }
    }
}
