﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HearXR.Audiobread
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SoundDefinition), true)]
    public class SoundDefinitionEditor : Editor
    {
        private SoundDefinition _soundDefinition;

        private string _assetPath;
        protected Rect _position;
        protected SerializedProperty _wasChangedProperty;
        protected List<SoundModule> _compatibleModules;
        private int _lastValidatedTime = -1;
        private const int _validationCooldown = 3;
        
        protected virtual void OnEnable()
        {
            _soundDefinition = (SoundDefinition) target;
            _assetPath = AssetDatabase.GetAssetPath(_soundDefinition);
            _wasChangedProperty = serializedObject.FindProperty("wasChanged");
            _compatibleModules = _soundDefinition.GetCompatibleModules();

            ValidateRequiredModules();
        }

        public override void OnInspectorGUI()
        {
            // _selectedTab = GUILayout.Toolbar(_selectedTab, new [] {"Main", "Everything else"});
            
            _position = EditorGUILayout.GetControlRect();
            
            var position = _position;
        
            serializedObject.Update();

            DrawModuleButtons();

            EditorGUI.BeginChangeCheck();

            DrawMainInspector();
            
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();

                // Remove enabled modules as needed.
                _soundDefinition.RescanEnabledModules();
                
                _wasChangedProperty.boolValue = true;
                serializedObject.ApplyModifiedProperties();
            }
        }

        protected virtual void DrawMainInspector()
        {
            // Show default inspector property editor
            DrawDefaultInspector();
        }

        protected void DrawModuleButtons()
        {
            EditorGUI.BeginChangeCheck();
            
            _soundDefinition.RescanEnabledModules();
            
            foreach (var module in _compatibleModules)
            {
                // Ignore default modules if they're enabled.
                // (Sometimes a default module doesn't get created by Unity, so this way we can still add it manually).
                if (module.EnabledByDefault && _soundDefinition.ModuleEnabled(module)) continue;

                if (_soundDefinition.ModuleEnabled(module))
                {
                    if (GUILayout.Button($"Remove {module.DisplayName}"))
                    {
                        _soundDefinition.RemoveModule(module);
                    }
                }
                else
                {
                    if (GUILayout.Button($"Add {module.DisplayName}"))
                    {
                        _soundDefinition.AddModule(module);
                    }
                }
            }
            
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                _soundDefinition.RescanEnabledModules();
            }
        }

        private void ValidateRequiredModules()
        {
            // Don't do anything if the asset hasn't been created yet.
            if (string.IsNullOrEmpty(_assetPath)) return;

            var currentTime = AudiobreadEditorUtilities.GetCurrentTime();

            if (_lastValidatedTime > 0 && (currentTime - _lastValidatedTime < _validationCooldown)) return;
            
            if (AddRequiredModulesToSoundDefinition(_soundDefinition))
            {
                serializedObject.ApplyModifiedProperties();
                _soundDefinition.RescanEnabledModules();
                _soundDefinition.OnDefaultModulesAdded();
            }
            
            _lastValidatedTime = currentTime;
        }
        
        protected bool AddRequiredModulesToSoundDefinition(SoundDefinition soundDefinition)
        {
            var addedModules = false;
            
            lock (soundDefinition)
            {
                var compatibleModules = soundDefinition.GetCompatibleModules();
                    
                foreach (var module in compatibleModules)
                {
                    // Add modules that should be enabled by default.
                    if (!module.EnabledByDefault) continue;
                    if (soundDefinition.ModuleEnabled(module)) continue;
                        
                    var soundModuleDefinition = soundDefinition.AddModule(module);
                    if (soundModuleDefinition != null)
                    {
                        addedModules = true;
                    }
                }
            
                if (addedModules)
                {
                    soundDefinition.OnDefaultModulesAdded();
                }
            }

            return addedModules;
        }
    }
}
