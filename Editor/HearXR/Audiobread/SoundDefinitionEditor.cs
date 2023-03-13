using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HearXR.Audiobread
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SoundDefinition), true)]
    public class SoundDefinitionEditor : Editor
    {
        private SoundDefinition _soundDefinition;

        // private int _selectedTab;

        private string _assetPath;
        protected Rect _position;
        protected SerializedProperty _wasChangedProperty;
        protected List<SoundModule> _compatibleModules;
        private static int _lastValidatedTime = -1;
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

            // Make a copy of the sound module properties, in case the user removes something.
            // This way we can delete it from the Asset file.
            var soundModuleSoundDefinitionsCopy = new List<SoundModuleDefinition>();
            for (int i = 0; i < _soundDefinition.ModuleSoundDefinitions.Count; ++i)
            {
                soundModuleSoundDefinitionsCopy.Add(_soundDefinition.ModuleSoundDefinitions[i]);
            }
            
            DrawMainInspector();
            
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                
                // Delete removed modules from the Asset.
                for (int i = 0; i < soundModuleSoundDefinitionsCopy.Count; ++i)
                {
                    var smp = soundModuleSoundDefinitionsCopy[i];
                    if (!_soundDefinition.ModuleSoundDefinitions.Contains(smp))
                    {
                        RemoveModuleFromAsset(smp);
                    }
                }

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
                        // TODO: Use bool / out combo
                        var smp = _soundDefinition.RemoveModule(module);
                        if (smp != null)
                        {
                            RemoveModuleFromAsset(smp);   
                        }
                    }
                }
                else
                {
                    if (GUILayout.Button($"Add {module.DisplayName}"))
                    {
                        // TODO: Use bool / out combo
                        var smp = _soundDefinition.AddModule(module);
                        if (smp != null)
                        {
                            AddModuleToAsset(smp);   
                        }
                    }
                }
            }
            
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                _soundDefinition.RescanEnabledModules();
            }
        }

        private int GetCurrentTime()
        {
            var t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
            return (int) t.TotalSeconds;
        }
        
        private void ValidateRequiredModules()
        {
            // Don't do anything if the asset hasn't been created yet.
            if (string.IsNullOrEmpty(_assetPath)) return;

            int currentTime = GetCurrentTime();

            if (_lastValidatedTime > 0 && (currentTime - _lastValidatedTime < _validationCooldown)) return;

            lock (_soundDefinition)
            {
                var addedModule = false;
                
                foreach (var module in _compatibleModules)
                {
                    // Add modules that should be enabled by default.
                    if (!module.EnabledByDefault) continue;
                    if (_soundDefinition.ModuleEnabled(module)) continue;
                
                    // Debug.Log($"Should enable module {module}");
                    var smp = _soundDefinition.AddModule(module);
                    if (smp != null)
                    {
                        addedModule = true;
                        AddModuleToAsset(smp, false);   
                    }
                }

                if (addedModule)
                {
                    // Debug.Log("Added modules. Save the asset.");
                    AssetDatabase.SaveAssets();

                    serializedObject.ApplyModifiedProperties();
                    _soundDefinition.RescanEnabledModules();
                    _soundDefinition.OnDefaultModulesAdded();
                }

                _lastValidatedTime = currentTime;
            }
        }
        
        protected void AddModuleToAsset(SoundModuleDefinition soundModuleDefinition, bool saveAsset = true)
        {
            AssetDatabase.AddObjectToAsset(soundModuleDefinition, _assetPath);
            if (saveAsset)
            {
                AssetDatabase.SaveAssets();
            }
            OnModuleAdded(ref soundModuleDefinition);
        }
        
        protected void RemoveModuleFromAsset(SoundModuleDefinition smp)
        {
            var smpAsset = AssetDatabase.LoadAssetAtPath(_assetPath, smp.GetType());
            AssetDatabase.RemoveObjectFromAsset(smpAsset);
            DestroyImmediate(smp);
            AssetDatabase.SaveAssets();
        }

        protected virtual void OnModuleAdded(ref SoundModuleDefinition soundModuleDefinition) {}
    }
}
