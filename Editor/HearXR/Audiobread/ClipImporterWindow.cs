using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HearXR.Audiobread
{
    public class ClipImporterWindow : EditorWindow
    {
        #region Private Fields
        private Vector2 _scrollPosition;
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
            var evt = Event.current;
            var dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
            
            GUI.Box(dropArea, "Drag audio clips here to create Audiobread clips");
     
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
                            CreateAudiobreadClips(audioClips);   
                        }
                    }
                    break;
            }
        }

        private static void CreateAudiobreadClips(IReadOnlyList<AudioClip> clips)
        {
            var selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            var folder = Path.GetDirectoryName(selectedPath);
            
            for (var i = 0; i < clips.Count; ++i)
            {
                // TODO: Create default Unity Core module as part of this asset.
                var audiobreadClip = CreateInstance<AudiobreadClipDefinition>();
                audiobreadClip.AudioClip = clips[i];

                var assetName = clips[i].name + "_AB_Clip.asset";
                var path = Path.Combine(folder, assetName);

                AssetDatabase.CreateAsset(audiobreadClip, path);
                Debug.Log($"Created new AudiobreadClip at {path}");
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        #endregion
    }
}
