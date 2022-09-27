using UnityEditor;
using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    // TODO: From Audiobread-ONE
    // /// <summary>
    // /// This property drawer displays FloatSoundPropertyDefinition in a way that allows the user to populate values.
    // /// </summary>
    // [CustomPropertyDrawer(typeof(FloatSoundPropertyValuesAttribute))]
    // public class SoundFloatControllerDrawer : PropertyDrawer
    // {
    //     private bool _showRandomization = true;
    //     
    //     public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    //     {
    //         int numLines = _showRandomization ? 3 : 1;
    //         return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * numLines + 10.0f;
    //     }
    //     
    //     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //     {
    //         // TODO: Validate property type. Must be FloatSoundPropertyDefinition.
    //         FloatSoundPropertyValuesAttribute attributeSettings = (FloatSoundPropertyValuesAttribute) attribute;
    //
    //         // TODO: Make the path a static
    //         BuiltInData builtInData = BuiltInData.Instance;
    //         if (builtInData == null)
    //         {
    //             // TODO: Better error handling.
    //             return;
    //         }
    //         
    //         
    //         // DisplaySoundProperty(ref property, ref builtInData, ref position, typeof(Volume), "volumeDefinition");
    //         
    //         FloatSoundProperty soundProperty = builtInData.properties.GetSoundPropertyByType<Volume>();
    //         Debug.LogError("This will certainly not work!!!!!");
    //
    //         _showRandomization = attributeSettings.showRandomization && soundProperty.Randomizable;
    //         
    //         SerializedProperty valueProp = property.FindPropertyRelative("floatValue");
    //         SerializedProperty randomizeProp = property.FindPropertyRelative("randomize");
    //         SerializedProperty varianceProp = property.FindPropertyRelative("variance");
    //         SerializedProperty soundPropertyProp = property.FindPropertyRelative("floatSoundProperty");
    //
    //         // TODO: Check sound property for null.
    //         if (soundPropertyProp.objectReferenceValue == null)
    //         {
    //             
    //             // TODO: Cannot assign this.....
    //             //soundPropertyProp.objectReferenceValue = soundProperty;
    //             valueProp.floatValue = soundProperty.DefaultValue;
    //         }
    //
    //         position.height = EditorGUIUtility.singleLineHeight;
    //         position.y += EditorGUIUtility.standardVerticalSpacing;
    //         
    //         EditorGUI.Slider(position, valueProp, soundProperty.MinLimit, soundProperty.MaxLimit, soundProperty.name);
    //         position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
    //         position.height = EditorGUIUtility.singleLineHeight;
    //
    //         if (_showRandomization)
    //         {
    //             EditorGUI.PropertyField(position, randomizeProp, true);
    //             position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
    //         
    //             EditorGUI.Slider(position, varianceProp, 0.0f, (soundProperty.MaxLimit - soundProperty.MinLimit));
    //             position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
    //         }
    //
    //         // TODO: Do I need to save anything here?
    //         
    //         
    //         
    //         
    //         
    //         
    //         
    //         
    //         
    //     }
    // }   
}
