using System.Collections.Generic;
using UnityEngine;

namespace HearXR.Audiobread
{
    // Wraps the sound.
    public class AudiobreadClipWrapper : SoundGeneratorWrapperUnityAudio<AudiobreadClipDefinition, AudiobreadClipWrapper, AudiobreadClip>
    {
        #region Private Fields
        private readonly AudiobreadPool _audiobreadPool;
        private double _virtualBeforeEndTime = -1;
        private bool _checkVirtualBeforeEnd;
        private double _virtualEndTime = -1;
        private bool _checkVirtualEnd;
        #endregion

        #region Constructor
        internal AudiobreadClipWrapper()
        {
#if UNITY_EDITOR
            _audiobreadPool = !Application.isPlaying ? AudiobreadPool.Instance : Audiobread.Instance.Pool;
#else
            _audiobreadPool = Audiobread.Instance.Pool;
#endif
        }
        #endregion

        #region Sound Abstract Methods
        protected override void DoStop(StopSoundFlags stopSoundFlags)
        {
            base.DoStop(stopSoundFlags);
            if (IsVirtual())
            {
                UnsetVirtual();
            }
        }
        #endregion

        #region Sound Method Overrides
        internal override void DoUpdate()
        {
            // TODO: Why are we totally overriding this?
            if (IsVirtual())
            {
                if (_checkVirtualBeforeEnd)
                {
                    CheckIfVirtualBeforeEnded();
                }

                if (_checkVirtualEnd)
                {
                    CheckIfVirtualEnded();
                }
            }

            UpdateSoundMultiple(_generators);
            _primedGenerator?.Update();

#if UNITY_EDITOR
            if (_soundDefinition.WasChanged)
            {
                RefreshSoundDefinition();
            }
#endif
        }
        #endregion

        #region SoundGeneratorWrapper Overrides
        protected override void DoOnSelfEnded()
        {
            base.DoOnSelfEnded();
            if (IsVirtual())
            {
                Debug.LogError("Something is happening with a virtual sound. Not sure what to do about that.");
            }
        }

        protected override IReadOnlyList<ISound> GetGenerators()
        {
            return _generators;
        }
        #endregion
        
        #region Private Methods
        protected override void PostPlaybackStateChanged(PlaybackState previousState, PlaybackState newState)
        {
            if (newState == PlaybackState.Stopped && IsVirtual())
            {
                newState = PlaybackState.Playing;
            }
            SetPlaybackState(newState);
        }

        private void PreReleasePlayer(AudiobreadClip generator)
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
            //return $"- CLIP - [{_soundDefinition.AudioClip.name}]";
            return $"- CLIP - [{_soundDefinition.name}]";
        }
        #endregion

        #region Status Check Methods
        private void SetVirtual(double projectedEndTime)
        {
            if (IsVirtual())
            {
                Debug.LogError("Well shit. We're already virtual. What now?");
            }

            // If projected end time has already passed, announce that clip has ended.
            if (AudioSettings.dspTime >= projectedEndTime)
            {
                UpdatePlaybackState();
                if (PlaybackState != PlaybackState.Stopped) return;
                InvokeOnEnded(this);
                return;
            }

            // TODO: Swap out the "Player" with a spoofed AudiobreadClipPlayer, which will pretend to be a real thing.
            // Otherwise, go into virtual mode, and fake clip behavior.
            SetStatus(SoundStatus.Virtual, true);

            _virtualEndTime = projectedEndTime;
            _virtualBeforeEndTime = _virtualEndTime - SCHEDULING_BUFFER;

            if (AudioSettings.dspTime < _virtualBeforeEndTime)
            {
                _checkVirtualBeforeEnd = true;
                _checkVirtualEnd = false;
                CheckIfVirtualBeforeEnded();
                return;
            }

            _checkVirtualBeforeEnd = false;
            _virtualBeforeEndTime = -1;

            _checkVirtualEnd = true;
            CheckIfVirtualEnded();
        }

        private void UnsetVirtual()
        {
            _checkVirtualEnd = false;
            _checkVirtualBeforeEnd = false;
            _virtualEndTime = -1;
            _virtualBeforeEndTime = -1;
            SetStatus(SoundStatus.Virtual, false);
        }

        private void CheckIfVirtualBeforeEnded()
        {
            if (AudioSettings.dspTime >= _virtualBeforeEndTime)
            {
                UpdatePlaybackState();
                InvokeOnBeforeEnded(this, _virtualEndTime);
                _checkVirtualBeforeEnd = false;
                _virtualBeforeEndTime = -1;
                _checkVirtualEnd = true;
                CheckIfVirtualEnded();
            }
        }

        private void CheckIfVirtualEnded()
        {
            if (AudioSettings.dspTime >= _virtualEndTime)
            {
                UnsetVirtual();
                PlaybackState previousState = PlaybackState;
                UpdatePlaybackState();
                if (PlaybackState == previousState) return;
                if (PlaybackState != PlaybackState.Stopped) return;
                InvokeOnEnded(this);
            }
        }
        #endregion

        #region SoundGeneratorWrapperUnityAudio Methods
        protected override void DoHandleStealRequested(AudiobreadClip generator, double projectedEndTime)
        {
            bool myChild = false;
            bool playingClip = false;

            // Check if this is our source.
            myChild = generator == _primedGenerator || _generators.Contains(generator);

            // Check if this source is playing.
            playingClip = generator.IsPlayingOrTransitioning();

            if (!myChild)
            {
                // TODO: This might happen. Just brute-force it somehow?
                Debug.LogError($"WHAT. THE. HELL!? " +
                               $"I am [{Guid.ToString().Substring(0, 4)}] " +
                               $"and [{generator.Guid.ToString().Substring(0, 4)}] is not in the list of my children.");
                return;
            }

            PreReleasePlayer(generator);

            // TODO: As far as the parent is concerned, I guess they shouldn't care.
            // TODO: Figure out how to handle stealing a playing source.
            // TODO: We should take over the scheduling jobs (or remember where we were in this source).
            // TODO: Sign up to receive the next source as soon as possible.
            if (playingClip)
            {
                SetVirtual(projectedEndTime);
            }

            UpdatePlaybackState();
        }
        
        protected override bool TryGetNewGenerator()
        {
            if (!_audiobreadPool.TryGetAudioSource(out AudiobreadSource source))
            {
                Debug.Log($"HEAR XR: {this} unable to get new source from pool");
                return false;
            }

            // TODO: Rename all the "player" references to "generator" for consistency.
            var player = source.Player;

            if (!player.IsValid()) return false;

            ((ISoundInternal<AudiobreadClipDefinition>) player).Init(_soundDefinition);
            player.SoundSourceObject = _soundSourceObject;
            
            player.ParentSound = _parentSound;
            //player.ParentSound = this;

            if (!player.IsValid()) return false;

            SubscribeToGeneratorEvents(ref player);

            _primedGenerator = player;
            return true;
        }
        
        protected override void ReleaseGenerator(AudiobreadClip generator)
        {
            PreReleasePlayer(generator);
            _audiobreadPool.ReturnAudioSource(generator.Source);
        }
        #endregion
    }
}