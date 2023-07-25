using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HearXR.Audiobread
{
    public static class AudiobreadEditorUtilities
    {
        internal static List<string> PATH_CHUNKS = new List<string> {"Assets", "AudiobreadData", "Resources"};
        
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

        /// <summary>
        /// Creates and returns a new audiobread serialized file.
        /// </summary>
        /// <param name="copyFrom">Optional data to copy values from.</param>
        /// <typeparam name="T">Type of scriptable object.</typeparam>
        /// <returns>Reference to newly created audiobread asset.</returns>
        internal static T CreateNewAudiobreadAsset<T>(T copyFrom = null) where T : ScriptableObject
        {
            T asset = default;

#if UNITY_EDITOR
            var type = typeof(T).ToString();
            var className = ClassNameFromType(type);

            var defaultFileName = (copyFrom == null) ? $"New{className}" : $"{copyFrom.name} copy";

            // Show the file naming creation popup.
            var filePath = EditorUtility.SaveFilePanelInProject("Enter file name", defaultFileName, "asset",
                "Enter new file name, but keep the path the same.", FolderFromType(type));

            // TODO: Handle errors.
            if (!string.IsNullOrEmpty(filePath))
            {
                if (copyFrom == null)
                {
                    asset = ScriptableObject.CreateInstance<T>();
                }
                else
                {
                    asset = Object.Instantiate(copyFrom);
                }

                AssetDatabase.CreateAsset(asset, filePath);
            }
#endif

            return asset;
        }
        
        /// <summary>
        /// Type name might be fully-qualified.
        /// Return just the actual class name.
        /// </summary>
        /// <param name="type">Type, possibly fully-qualified</param>
        /// <returns>Single class name (not fully-qualified)</returns>
        private static string ClassNameFromType(string type)
        {
            var typeChunks = type.Split('.');
            return typeChunks[typeChunks.Length - 1];
        }
        
        /// <summary>
        /// Returns asset folder from type name. 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string FolderFromType(string type)
        {
            var className = ClassNameFromType(type);
            var path = Path.Combine(Path.Combine(PATH_CHUNKS.ToArray()), className);
            CreateAssetFolderIfDoesNotExist(path);
            return path;
        }

        private static void CreateAssetFolderIfDoesNotExist(string directoryPath)
        {
            if (Directory.Exists(directoryPath)) return;
            Directory.CreateDirectory(directoryPath);
            AssetDatabase.Refresh();
        }
    }
}