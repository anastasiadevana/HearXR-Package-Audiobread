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
            
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Width(position.width));

            EditorGUILayout.LabelField("Audiobread Clip Importer");
            
            AudioClipsDropArea();

            EditorGUILayout.EndScrollView();
        }
        #endregion
        
        #region Private Methods
        private void AudioClipsDropArea()
        {
            // Display folder picker.
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

            // Display drag and drop area.
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
                var audiobreadClip = CreateInstance<AudiobreadClipDefinition>();
                audiobreadClip.AudioClip = clips[i];

                var assetName = clips[i].name + "_AB_Clip.asset";
                var path = Path.Combine(_importFolder, assetName);

                AssetDatabase.CreateAsset(audiobreadClip, path);
                Debug.Log($"Created new AudiobreadClip at {path}");
                yield return null;
                
                AudiobreadEditorUtilities.AddRequiredModulesToSoundDefinition(audiobreadClip, path);
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        #endregion
    }
}
