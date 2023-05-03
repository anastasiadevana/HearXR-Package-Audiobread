using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

// TODO: Add an option to enable default modules.
// TODO: Add an option to set up bulk starting setting for the imported clips.
namespace HearXR.Audiobread
{
    public class ClipImporterWindow : EditorWindow
    {
        #region Private Fields
        private Vector2 _scrollPosition;
        private string _importFolder = "Assets";
        private bool _userSelectedFolder = false;
        private readonly string[] _creationOptions = {"Audiobread Clip", "Simple Sampler"};
        private int _definitionTypeIndex = 0;
        #endregion
        
        #region Unity Editor Methods
        [MenuItem("🔈 Audiobread  🔈/Clip Importer")]
        public static void ShowWindow()
        {
            GetWindow(typeof(ClipImporterWindow));
        }

        private void OnGUI()
        {
            titleContent = new GUIContent("Audiobread Clip Importer");
            
            EditorGUILayout.BeginVertical();
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Width(position.width));

            DisplayWindowContent();
            
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
        #endregion
        
        #region Private Methods
        private void DisplayWindowContent()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Audiobread Clip Importer");
            
            // Display folder picker.
            EditorGUILayout.Space();
            var startingPath = AudiobreadEditorUtilities.GetSelectedDirectory();
            if (!_userSelectedFolder)
            {
                _importFolder = startingPath;   
            }
            
            if (GUILayout.Button("Browse to folder...", GUILayout.Width(200)))
            {
                
                var selectedPath = EditorUtility.OpenFolderPanel("Browse to folder", startingPath, "");
                
                //User selected a folder.
                if (selectedPath.Length != 0)
                {
                    var firstPosition = selectedPath.IndexOf("Assets", StringComparison.Ordinal);
                    if (firstPosition >= 0)
                    {
                        selectedPath = selectedPath.Substring(firstPosition);
                        _importFolder = selectedPath;
                        _userSelectedFolder = true;   
                    }
                }
            }
            EditorGUILayout.LabelField($"Target folder: {_importFolder}");

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Import type: ");
            _definitionTypeIndex = EditorGUILayout.Popup(_definitionTypeIndex, _creationOptions);

            // Display drag and drop area.
            EditorGUILayout.Space();
            var evt = Event.current;
            var dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
            
            GUI.Box(dropArea, "Drag audio clips here to create Audiobread clips");

            // User dragged some clips.
            switch (evt.type) {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition))
                    {
                        return;   
                    }

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
         
                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        var audioClips = new List<AudioClip>();
                        for (var i = 0; i < DragAndDrop.objectReferences.Length; i++)
                        {
                            var draggedObject = DragAndDrop.objectReferences[i];
                            if (draggedObject is AudioClip clip)
                            {
                                audioClips.Add(clip);
                            }
                        }

                        if (audioClips.Count > 0)
                        {
                            EditorCoroutineUtility.StartCoroutineOwnerless(CreateAudiobreadClips(audioClips));
                        }
                    }
                    break;
            }
        }
        
        private IEnumerator CreateAudiobreadClips(IReadOnlyList<AudioClip> clips)
        {
            // TODO: I should just use this in the GetSelectedDirectory method I think
            // var selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            // var folder = Path.GetDirectoryName(selectedPath);

            for (var i = 0; i < clips.Count; ++i)
            {
                SoundDefinition soundDefinition = default;

                var assetName = "";
                var assetType = "";
                
                if (_definitionTypeIndex == 0)
                {
                    var audiobreadClipDefinition = CreateInstance<AudiobreadClipDefinition>();
                    audiobreadClipDefinition.AudioClip = clips[i];
                    soundDefinition = audiobreadClipDefinition;
                    assetName = clips[i].name + "_AB_Clip.asset";
                    assetType = nameof(AudiobreadClipDefinition);
                }
                else
                {
                    var simpleSamplerDefinition = CreateInstance<SimpleSamplerDefinition>();
                    simpleSamplerDefinition.AudioClip = clips[i];
                    soundDefinition = simpleSamplerDefinition;
                    assetName = clips[i].name + "_SimpleSampler.asset";
                    assetType = nameof(SimpleSamplerDefinition);
                }
                
                var path = Path.Combine(_importFolder, assetName);

                AssetDatabase.CreateAsset(soundDefinition, path);
                Debug.Log($"Created new {assetType} at {path}");
                yield return null;
                
                AudiobreadEditorUtilities.AddRequiredModulesToSoundDefinition(soundDefinition, path);
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        #endregion
    }
}
