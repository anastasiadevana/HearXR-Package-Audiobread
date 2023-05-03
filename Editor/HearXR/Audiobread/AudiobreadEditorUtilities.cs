using System;
using System.IO;
using UnityEditor;
using Object = UnityEngine.Object;

namespace HearXR.Audiobread
{
    public static class AudiobreadEditorUtilities
    {
        internal static int GetCurrentTime()
        {
            var t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return (int) t.TotalSeconds;
        }
        
        internal static string GetSelectedDirectory(string defaultPath = "Assets")
        {
            var path = defaultPath;

            foreach (var obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }
            return path;
        }

        internal static bool AddRequiredModulesToSoundDefinition(SoundDefinition soundDefinition, string assetPath)
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
                        AssetDatabase.AddObjectToAsset(soundModuleDefinition, assetPath);
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