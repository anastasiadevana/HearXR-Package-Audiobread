using UnityEngine;

namespace HearXR.Audiobread
{
    public abstract class SoundGeneratorWrapperUnityAudio<TDefinition, TSelf, TGenerator> : SoundGeneratorWrapper<TDefinition, TSelf, TGenerator>, ISoundGeneratorWrapper
        where TDefinition : class, ISoundDefinition
        where TSelf : Sound
        where TGenerator : Sound, ISoundGeneratorUnityAudio
    {
        #region Properties
        // TODO: Adding this here so that we can access the meter on the AudioSource.
        //       However, this is not the cleanest way to do this.
        //       Should we have another method that provides that reference as part of the Play event?
        //       Or should we expose a list of children maybe with timestamps of when they were last played?
        //       Or maybe we add this as part of the callback?
        public TGenerator LastPlayedGenerator => _lastPlayedGenerator;
        #endregion
        
        #region Sound Abstract Methods
        protected override bool CanPlay(PlaySoundFlags playSoundFlags = PlaySoundFlags.None)
        {
            if (_primedGenerator != null)
            {
                if (!_primedGenerator.IsValid())
                {
                    Debug.LogWarning($"HEAR XR: Unable to play {this} because player is invalid.");
                    return false;
                }

                return true;
            }

            // TODO: If we schedule far in advance, the sound source may get stolen, or something else can happen in the game.
            // TODO: If start time is fairly far away, maybe just wait for it, instead of scheduling this far in advance.
            // TODO: In that case, we don't need to prepare the player here, just yet. Prepare it when it gets close to playing time.

            if (PrepareGenerator() && _primedGenerator.IsValid())
            {
                return true;
            }

            Debug.LogWarning($"HEAR XR: Unable to play {this} because unable to prepare player.");
            return false;
        }
        
        protected override void DoPlay(PlaySoundFlags playFlags, bool scheduled, double startTime = -1.0d)
        {
            // TODO: If we schedule this far in advance, the sound source may get stolen, or something else can happen in the game.
            // TODO: If start time is fairly far away, maybe just wait for it, instead of scheduling this far in advance.

            var generator = _primedGenerator;
            _primedGenerator = default(TGenerator);

            _generators.Add(generator);

            if (scheduled)
            {
                generator.PlayScheduled(startTime, playFlags);
            }
            else
            {
                generator.Play(playFlags);
            }

            _lastPlayedGenerator = generator;

            SetStatus(SoundStatus.Paused, false);
        }
        
        protected override void DoReleaseResources()
        {
            // Do not call base, because the player gets released in a different way.
            for (var i = _generators.Count - 1; i >= 0; --i)
            {
                ReleaseGenerator(_generators[i]);
            }

            if (_primedGenerator != null)
            {
                ReleaseGenerator(_primedGenerator);
            }
        }
        #endregion
        
        #region Protected Methods
        protected void SubscribeToGeneratorEvents(ref TGenerator player)
        {
            player.OnStealRequested += HandleStealRequested;
            player.OnBegan += OnChildBegan;
            player.OnEnded += OnChildEnded;
            player.OnScheduled += OnChildScheduled;
            player.OnBeforeEnded += OnChildBeforeEnded;
        }

        protected void UnsubscribeFromGeneratorEvents(ref TGenerator player)
        {
            player.OnStealRequested -= HandleStealRequested;
            player.OnBegan -= OnChildBegan;
            player.OnEnded -= OnChildEnded;
            player.OnScheduled -= OnChildScheduled;
            player.OnBeforeEnded -= OnChildBeforeEnded;
        }
        #endregion

        #region Events
        protected void HandleStealRequested(ISoundGeneratorUnityAudio generator, double projectedEndTime)
        {
            DoHandleStealRequested((TGenerator) generator, projectedEndTime);
        }
        #endregion
        
        #region Abstract Methods
        protected abstract void DoHandleStealRequested(TGenerator generator, double projectedEndTime);
        protected abstract bool TryGetNewGenerator();
        protected abstract void ReleaseGenerator(TGenerator generator);
        #endregion
        
        #region Private Methods
        private bool PrepareGenerator()
        {
            if (HavePrimedGenerator())
            {
                Debug.Log("We have prime!");
                return true;
            }

            if (!TryUseExistingGenerator())
            {
                if (!TryGetNewGenerator())
                {
                    return false;
                }
            }

            if (!HavePrimedGenerator() || !_primedGenerator.IsValid())
            {
                Debug.LogError($"HEAR XR: {this} For some reason, {this}'s new source is no good :(");
                return false;
            }
            
            ((ISoundInternal) _primedGenerator).PrepareToPlay();
            return true;
        }
        #endregion
    }
}