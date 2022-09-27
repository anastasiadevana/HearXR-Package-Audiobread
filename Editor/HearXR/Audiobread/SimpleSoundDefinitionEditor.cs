using UnityEngine;
using UnityEditor;
using UnityEngine.PlayerLoop;

namespace HearXR.Audiobread
{
    // TODO: From Audiobread-ONE
    // //[CanEditMultipleObjects, CustomEditor(typeof(SimpleSoundDefinition), true)]
    // public class SimpleSoundDefinitionEditor : Editor
    // {
    //     // // TODO: Figure out the minimum number of sound sources needed based on the shortest clips.
    //     //
    //     // //private GameObject _previewGO;
    //     // private AudiobreadClipPlayer[] _previewSources;
    //     // private SimpleSoundDefinition _soundDefinition;
    //     // private SimpleSound _simpleSound;
    //     // private ParentSoundType _typeWhenStarted;
    //     // private int _clipsWhenStarted;
    //     // private AudiobreadPool _audiobreadPool;
    //     // private SoundRegistry _soundRegistry;
    //     //
    //     // void OnEnable()
    //     // {
    //     //     _audiobreadPool = AudiobreadPool.Instance;
    //     //     if (!_audiobreadPool.HasPool)
    //     //     {
    //     //         _audiobreadPool.TryInitEditorPool(100, 0);
    //     //     }
    //     //     EditorApplication.update += OnUpdate;
    //     // }
    //     //
    //     // void OnUpdate()
    //     // {
    //     //     // TODO: There should be public functions on SoundBase (IsPlaying(), etc)
    //     //     if (_simpleSound != null /*&& SoundBase.IsSoundPlaying(_simpleSound.Status)*/)
    //     //     {
    //     //         EditorApplication.QueuePlayerLoopUpdate();
    //     //         _simpleSound.Update();
    //     //     }
    //     // }
    //     //
    //     // void OnDisable()
    //     // {
    //     //     if (_simpleSound != null)
    //     //     {
    //     //         _simpleSound.Stop(StopSoundFlags.Instant | StopSoundFlags.UnsetPersistentFlag);
    //     //     }
    //     //     if (_audiobreadPool != null)
    //     //     {
    //     //         _audiobreadPool.ClearEditorPool();   
    //     //     }
    //     //     if (_soundRegistry != null)
    //     //     {
    //     //         _soundRegistry.ClearRegistry();   
    //     //     }
    //     // }
    //     //
    //     // public override void OnInspectorGUI()
    //     // {
    //     //     DrawDefaultInspector();
    //     //
    //     //     _soundDefinition = (SimpleSoundDefinition) target;
    //     //     EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);
    //     //     GUILayout.Space(20);
    //     //     Rect r = EditorGUILayout.BeginHorizontal();
    //     //     if (_soundDefinition.children.Count > 0)
    //     //     {
    //     //         if (GUILayout.Button("Preview"))
    //     //         {
    //     //             if (_simpleSound != null)
    //     //             {
    //     //                 _simpleSound.Stop(StopSoundFlags.Instant | StopSoundFlags.UnsetPersistentFlag);
    //     //                 _simpleSound = null;
    //     //             }
    //     //             
    //     //             _simpleSound = (SimpleSound) _soundDefinition.CreateSound();
    //     //
    //     //             if (_simpleSound.IsValid())
    //     //             {
    //     //                 _typeWhenStarted = _soundDefinition.type;
    //     //                 _clipsWhenStarted = _soundDefinition.children.Count;
    //     //                 _simpleSound.Play();
    //     //             }
    //     //             else
    //     //             {
    //     //                 Debug.Log("Sound isn't valid :(");
    //     //             }
    //     //         }
    //     //     }
    //     //     
    //     //
    //     //     if (_simpleSound != null && _simpleSound.IsPlayingOrTransitioning())
    //     //     {
    //     //         if (GUILayout.Button("Stop"))
    //     //         {
    //     //             _simpleSound.Stop(StopSoundFlags.Instant | StopSoundFlags.UnsetPersistentFlag);
    //     //             _simpleSound = null;
    //     //         }
    //     //         
    //     //         // When something important changes, stop the sound.
    //     //         if (_typeWhenStarted != _soundDefinition.type || _clipsWhenStarted != _soundDefinition.children.Count)
    //     //         {
    //     //             if (_simpleSound.IsValid())
    //     //             {
    //     //                 _simpleSound.Stop(StopSoundFlags.Instant);   
    //     //             }
    //     //         }
    //     //     }
    //     //
    //     //     EditorGUILayout.EndHorizontal();
    //     //     EditorGUI.EndDisabledGroup();
    //     // }
    // }
}

