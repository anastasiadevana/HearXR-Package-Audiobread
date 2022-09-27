using System;
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace HearXR.Audiobread
{
    public class SoundDefinitionsWindow : EditorWindow
    {
        private Rect leftPanel;
        private Rect rightPanel;
        private Vector2 _scrollPosition;
        private Vector2 _scrollPosition2;
        private string[] _guids;
        private string[] _paths;
        private Editor _editor;
        private float _leftPanelWidth = 200;
        private float _panelSpacing = 20;
        private ScriptableObject _s;
        
        [MenuItem ("🔈 Audiobread  🔈/Sound Definitions")]
        public static void  ShowWindow () {
            EditorWindow.GetWindow(typeof(SoundDefinitionsWindow), false, "Sound Definitions");
        }

        private void OnEnable()
        {
            _guids = AssetDatabase.FindAssets("t:SoundDefinition", null);
            _paths = new string[_guids.Length];
            for (int i = 0; i < _guids.Length; ++i)
            {
                _paths[i] = AssetDatabase.GUIDToAssetPath(_guids[i]);
            }
        }

        void OnGUI ()
        {
            DrawLeftPanel();
            DrawRightPanel();
        }

        private void DrawLeftPanel()
        {
            leftPanel = new Rect(0, 0, _leftPanelWidth + _panelSpacing, position.height);
 
            GUILayout.BeginArea(leftPanel);
            
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Width(_leftPanelWidth));
            for (int i = 0; i < _guids.Length; ++i)
            {
                ScriptableObject s = (ScriptableObject) AssetDatabase.LoadAssetAtPath(_paths[i], typeof(ISoundDefinition));
                if (GUILayout.Button(s.name))
                {
                    EditorGUIUtility.PingObject(s);
                    Editor.CreateCachedEditor(s, null, ref _editor);
                    _s = s;
                }
            }
            EditorGUILayout.EndScrollView();
            
            GUILayout.EndArea();
        }

        private void DrawRightPanel()
        {
            rightPanel = new Rect(200, 0, position.width - _leftPanelWidth - _panelSpacing, position.height);
 
            GUILayout.BeginArea(rightPanel);
            
            if (_editor != null)
            {
                if (_s != null)
                {
                    EditorGUILayout.LabelField($"Editing: {_s.name}", EditorStyles.boldLabel);   
                }
                EditorGUIUtility.labelWidth = 250;
                _scrollPosition2 = EditorGUILayout.BeginScrollView(_scrollPosition2);
                _editor.OnInspectorGUI();
                EditorGUILayout.EndScrollView();
            }
            
            GUILayout.EndArea();
        }
    }
}