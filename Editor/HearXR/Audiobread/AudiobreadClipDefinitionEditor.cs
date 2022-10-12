using HearXR.Audiobread.SoundProperties;
using UnityEditor;
using UnityEngine;

namespace HearXR.Audiobread
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AudiobreadClipDefinition), true)]
    public class AudiobreadClipDefinitionEditor : SoundDefinitionEditor
    {
         // private bool _positionSet;
         
//         // public float GetPropertyHeight(SerializedProperty property, GUIContent label)
//         // {
//         //     int numLines = 1;
//         //     return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * numLines;
//         // }
         
         public void SetPosition(Rect position)
         {
             _position = position;
             // _positionSet = true;
         }
         
//         public override void OnInspectorGUI()
//         {
//             serializedObject.Update();
//             
//             // TODO: Use DelayedFloatField!!! Since this is changing too fast.
//             // TODO: Add horizontal group or what not.
//             BuiltInData builtInData = BuiltInData.Instance;
//             if (builtInData == null)
//             {
//                 // TODO: Better error handling.
//                 return;
//             }
//     
//             // TODO: This needs to happen in the SoundDefinition editor, since we don't want isPlaying to be serialized.
//             /*
//             SerializedProperty isPlaying = property.FindPropertyRelative("isPlaying");
//             Rect isPlayingRect = position;
//             isPlayingRect.width = 32;
//             isPlayingRect.x = 0;
//             string isPlayingMarker = isPlaying.boolValue ? ">" : "";
//             EditorGUI.LabelField(isPlayingRect, new GUIContent(isPlayingMarker));
//             */
//     
//             //Rect position = GUILayoutUtility.GetLastRect();
//             //position.height = EditorGUIUtility.singleLineHeight;
//     
//             if (!_positionSet)
//             {
//                 _position = EditorGUILayout.GetControlRect();
//             }
//     
//             Rect position = _position;
//             
//             //Rect 
//             //position.y -= EditorGUIUtility.singleLineHeight;
//             
//             //Rect position = EditorGUILayout.BeginHorizontal();
//     
//             SerializedProperty audioClipProp = serializedObject.FindProperty("_audioClip");
//             SerializedProperty wasChangedProp = serializedObject.FindProperty("wasChanged");
//             
//             float fieldWidth = position.width - EditorGUIUtility.labelWidth;
//             int subFields = 4;
//             float subFieldWidth = 70;
//             float totalSubfields = subFields * subFieldWidth;
//             float subFieldSpacing = Mathf.Max(Mathf.Floor((fieldWidth - totalSubfields) / 3), 0);
//             float subFieldWithSpace = subFieldWidth + subFieldSpacing;
//             
//             EditorGUI.BeginChangeCheck();
//             
//             position.x = 20;
//             position.width = EditorGUIUtility.labelWidth - 20;
//             EditorGUI.ObjectField(position, audioClipProp, new GUIContent(""));
//             //position.x += position.width + _horizontalSpacer + 20;
//             position.x = EditorGUIUtility.labelWidth + 5;
//     
//             
//             float initialX = position.x;
//             
//             DisplaySoundProperty(ref builtInData, ref position, typeof(Volume), "_volumeDefinition");
//             //position.x += position.width - 5;
//             position.x = initialX + subFieldWithSpace;
//     
//             DisplaySoundProperty(ref builtInData, ref position, typeof(Pitch), "pitchDefinition");
//             //position.x += position.width - 5;
//             position.x = initialX + subFieldWithSpace * 2;
//             
//             DisplaySoundProperty(ref builtInData, ref position, typeof(Offset), "offsetDefinition");
//             //position.x += position.width - 5;
//             position.x = initialX + subFieldWithSpace * 3;
//             
//             DisplaySoundProperty(ref builtInData, ref position, typeof(Delay), "delayDefinition");
//             
//             
//             /*
//              
//             // Some random testing with curves and curve swatches. 
//             
//             SerializedProperty curveProp = serializedObject.FindProperty("curve");
//             SerializedProperty curveProp2 = serializedObject.FindProperty("curve2");
//             AnimationCurve curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
//             AnimationCurve curve2 = new AnimationCurve(new Keyframe(0, 0.1f), new Keyframe(1.0f, 1.1f));
//
//             curveProp.animationCurveValue = EditorGUILayout.CurveField(curve, Color.magenta,
//                 new Rect(0, 0, 1, 1), GUILayout.Height(200.0f));
//
//             curveProp2.animationCurveValue =
//                 EditorGUILayout.CurveField(curve2, Color.cyan, new Rect(0, 0, 1, 1), GUILayout.Height(200.0f));
//
//             position = EditorGUILayout.GetControlRect();
//             position.height = 200.0f;
//
//             EditorGUILayout.BeginVertical(GUILayout.Height(200.0f));
//             EditorGUILayout.Space();
//             EditorGUILayout.EndVertical();
//             EditorGUIUtility.DrawRegionSwatch(position, curveProp, curveProp2, Color.white, Color.black, new Rect(0, 0, 1, 1));
//             */
//             
//             if (EditorGUI.EndChangeCheck())
//             {
//                 wasChangedProp.boolValue = true;
//             }
//             
//             serializedObject.ApplyModifiedProperties();
//         }
//     
//         private void DisplaySoundProperty(
//             ref BuiltInData builtInData, 
//             ref Rect position,
//             System.Type propertyType, 
//             string propertyFieldName)
//         {
//             FloatSoundProperty propertyInfo =
//                 (FloatSoundProperty) builtInData.properties.GetSoundPropertyByType(propertyType);
//     
//             SerializedProperty definitionProp = serializedObject.FindProperty(propertyFieldName);
//     
//             SerializedProperty valueProp = definitionProp.FindPropertyRelative("value");
//     
//             SerializedProperty userSelectedProp = definitionProp.FindPropertyRelative("userSelected");
//     
//             SerializedProperty soundPropertyProp = definitionProp.FindPropertyRelative("soundProperty");
//             //soundPropertyProp.objectReferenceValue = null;
//             if (soundPropertyProp.objectReferenceValue == null)
//             {
//                 soundPropertyProp.objectReferenceValue = propertyInfo;
//                 valueProp.floatValue = propertyInfo.DefaultValue;
//             }
//             else
//             {
//                 
//             }
//     
//             // TODO: Validate offset value based on clip length.
//             
//             //Debug.Log(soundPropertyProp.objectReferenceValue == null);
//             
//             // if (userSelectedProp.boolValue == false)
//             // {
//             //     valueProp.floatValue = propertyInfo.DefaultValue;
//             //     userSelectedProp.boolValue = true;
//             // }
//             
//             position.width = 36;
//             EditorGUI.LabelField(position, propertyInfo.ShortName);
//             position.x += position.width - 10;
//             position.width = 50;
//             
//             EditorGUI.BeginChangeCheck();
//             
//             float propertyFloatValue = valueProp.floatValue;
//             propertyFloatValue = EditorGUI.FloatField(position, propertyFloatValue);
//             
//             propertyFloatValue = Mathf.Clamp(propertyFloatValue, propertyInfo.MinLimit, propertyInfo.MaxLimit);
//             
//             if (EditorGUI.EndChangeCheck())
//             {
//                 valueProp.floatValue = propertyFloatValue;
//             }
//             
//             // TODO: Validate against allowed ranges.
//         }
    }
}