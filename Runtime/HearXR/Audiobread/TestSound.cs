using System.Collections.Generic;
using UnityEngine;

// TODO: DELETE THIS. NO LONGER USED.
namespace HearXR.Audiobread
{
    public class TestSound : SoundGeneratorWrapperUnityAudio<TestSoundDefinition, TestSound, TestAudiobreadClip>
    {
        private readonly AudiobreadPool _audiobreadPool;
        
        #region Constructor
        internal TestSound()
        {
#if UNITY_EDITOR
            _audiobreadPool = !Application.isPlaying ? AudiobreadPool.Instance : AudiobreadManager.Instance.Pool;
#else
            _audiobreadPool = AudiobreadManager.Instance.Pool;
#endif
        }
        #endregion

        protected override void DoHandleStealRequested(TestAudiobreadClip generator, double projectedEndTime)
        {
            throw new System.NotImplementedException();
        }

        protected override bool TryGetNewGenerator()
        {
            // TODO: Rename all the "player" references to "generator" for consistency.
            if (!_audiobreadPool.TryGetAudioSource(out AudiobreadSource source))
            {
                Debug.Log($"HEAR XR: {this} unable to get new source from pool");
                return false;
            }

            var player = source.TestPlayer;

            if (!player.IsValid()) return false;

            ((ISoundInternal<TestSoundDefinition>) player).Init(_soundDefinition);
            player.SoundSourceObject = _soundSourceObject;
            
            player.ParentSound = _parentSound;
            //player.ParentSound = this;

            if (!player.IsValid()) return false;

            SubscribeToTestEvents(ref player);

            _primedGenerator = player;
            return true;
        }

        protected override void InitModules(InitSoundFlags initSoundFlags = InitSoundFlags.None)
        {
            // TODO: Move to the Unity Generator Wrapper thingy.
            // Do nothing, because we only want to init on the TestAudioPlayer.
        }

        // protected override void ApplySoundDefinitionAndProperties(InitSoundFlags initSoundFlags = InitSoundFlags.None)
        // {
        //     base.ApplySoundDefinitionAndProperties(initSoundFlags);
        // }
        
        protected override void ReleaseGenerator(TestAudiobreadClip generator)
        {
            PreReleasePlayer(generator);
            _audiobreadPool.ReturnAudioSource(generator.Source);
        }
        
        private void PreReleasePlayer(TestAudiobreadClip generator)
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

        protected void SubscribeToTestEvents(ref TestAudiobreadClip player)
        {
            player.OnStealRequested += HandleStealRequested;
            player.OnBegan += OnChildBegan;
            player.OnEnded += OnChildEnded;
            player.OnScheduled += OnChildScheduled;
            player.OnBeforeEnded += OnChildBeforeEnded;
        }

        protected void UnsubscribeFromTestEvents(ref TestAudiobreadClip player)
        {
            player.OnStealRequested -= HandleStealRequested;
            player.OnBegan -= OnChildBegan;
            player.OnEnded -= OnChildEnded;
            player.OnScheduled -= OnChildScheduled;
            player.OnBeforeEnded -= OnChildBeforeEnded;
        }

        protected override IReadOnlyList<ISound> GetGenerators()
        {
            return _generators;
        }
    }
}