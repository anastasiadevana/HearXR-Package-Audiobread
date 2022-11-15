using System.Collections.Generic;
using UnityEngine;

namespace HearXR.Audiobread
{
     public class ToneGeneratorWrapper : SoundGeneratorWrapperUnityAudio<ToneGeneratorDefinition, ToneGeneratorWrapper, ToneGenerator>
     {
          #region Private Fields
          private readonly AudiobreadPool _audiobreadPool;
          #endregion
          
          #region Constructor
          internal ToneGeneratorWrapper()
          {
#if UNITY_EDITOR
               _audiobreadPool = !Application.isPlaying ? AudiobreadPool.Instance : Audiobread.Instance.Pool;
#else
               _audiobreadPool = Audiobread.Instance.Pool;
#endif
          }
          #endregion
          
          protected override IReadOnlyList<ISound> GetGenerators()
          {
               return _generators;
          }

          protected override void DoHandleStealRequested(ToneGenerator generator, double projectedEndTime)
          {
               bool mine;
               bool isGenerating;

               // Check if this is our source.
               mine = generator == _primedGenerator || _generators.Contains(generator);

               // Check if it's generating something or whatever.
               isGenerating = generator.IsPlayingOrTransitioning();

               if (!mine)
               {
                    // TODO: This might happen. Just brute-force it somehow?
                    Debug.LogError($"WHAT. THE. HELL!? " +
                                   $"I am [{Guid.ToString().Substring(0, 4)}] " +
                                   $"and [{generator.Guid.ToString().Substring(0, 4)}] is not in the list of my children.");
                    return;
               }

               PreReleaseGenerator(generator);

               // TODO: As far as the parent is concerned, I guess they shouldn't care.
               // TODO: Figure out how to handle stealing a playing source.
               // TODO: We should take over the scheduling jobs (or remember where we were in this source).
               // TODO: Sign up to receive the next source as soon as possible.
               if (isGenerating)
               {
                    // TODO: Handle virtual stuff?
                    // SetVirtual(projectedEndTime);
               }

               UpdatePlaybackState(); 
          }

          protected override bool TryGetNewGenerator() // Prepare to play from here
          {
               if (!_audiobreadPool.TryGetAudioSource(out var source))
               {
                    Debug.Log($"HEAR XR: {this} unable to get new source from pool");
                    return false;
               }

               var generator = source.Generator;

               if (!generator.IsValid()) return false;

               ((ISoundInternal<ToneGeneratorDefinition>) generator).Init(_soundDefinition);
               generator.SoundSourceObject = _soundSourceObject;
               generator.MidiNoteInfo = _midiNoteInfo;
               generator.ParentSound = _parentSound;

               if (!generator.IsValid()) return false;

               SubscribeToGeneratorEvents(ref generator);

               _primedGenerator = generator;
               return true;
          }

          protected override void ReleaseGenerator(ToneGenerator generator)
          {
               PreReleaseGenerator(generator);
               _audiobreadPool.ReturnAudioSource(generator.Source);
          }
          
          #region Private Methods
          private void PreReleaseGenerator(ToneGenerator generator)
          {
               if (generator.IsPlayingOrTransitioning())
               {
                    lock (_nonStoppedGenerators)
                    {
                         _nonStoppedGenerators.Remove(generator.Guid);
                    }
               }

               _generators.Remove(generator);
               if (generator == _primedGenerator)
               {
                    _primedGenerator = null;
               }

               generator.ReleaseResources();

               UnsubscribeFromGeneratorEvents(ref generator);
          }
          #endregion
          
          #region Helper Methods
          public override string ToString()
          {
               return $"- TONE GENERATOR WRAPPER";
          }
          #endregion
     }
}