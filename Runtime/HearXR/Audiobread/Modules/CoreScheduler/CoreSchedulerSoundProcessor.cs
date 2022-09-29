using HearXR.Audiobread.SoundProperties;
using UnityEngine;

namespace HearXR.Audiobread
{
    public class CoreSchedulerSoundProcessor : SoundModuleProcessor<CoreSchedulerSoundModule, CoreSchedulerSoundModuleDefinition>, IChildSchedulerProcessor
    {
        // TODO: We have a problem of where to store _childIndex.
        //       It is relevant to both one-shots and continuous sounds.
        //       But currently this module will not reset it if the sound is a one-shot.
        
        #region Private Fields
        private IContainerSoundDefinition _soundDefinition;
        private bool _enabled = false; // TODO: Do something about this?
        private int _childIndex = -1;
        private int _repeatsRemaining;
        private bool _limitedRepeats;
        private bool _playMore = true;
        #endregion
        
        #region Properties
        public int ChildIndex
        {
            get => _childIndex;
            set => _childIndex = value;
        }

        public bool PlayMore => _playMore;
        #endregion

        public CoreSchedulerSoundProcessor(CoreSchedulerSoundModule soundModule, ISound sound) : base(soundModule, sound) {}

        protected override void DoInit()
        {
            base.DoInit();
            if (MySound.SoundDefinition is IContainerSoundDefinition)
            {
                _enabled = true;
                _soundDefinition = MySound.SoundDefinition as IContainerSoundDefinition;
            }
        }

        protected override double ApplySoundModifiers(SetValuesType setValuesType, 
            PlaySoundFlags playSoundFlags = PlaySoundFlags.None, double startTime = Audiobread.INACTIVE_START_TIME)
        {
            return startTime;
        }

        internal bool CanPlay(PlaySoundFlags playSoundFlags = PlaySoundFlags.None)
        {
            // We only care about continuous sounds.
            if (_soundDefinition.SoundType != SoundType.Continuous) return true;
            
            // If this is the first time we're playing this sound. Let's init the scheduler values.
            if (!Sound.HasPlayFlag(playSoundFlags, PlaySoundFlags.PlayNext))
            {
                InitPlaybackValues();
            }
            
            return _playMore;
        }

        internal void ProcessSchedulingBeforePlay(PlaySoundFlags playFlags, ref bool scheduled, ref double startTime)
        {
            // We only care about continuous sounds.
            if (_soundDefinition.SoundType != SoundType.Continuous) return;
            
            // Always schedule, even if wasn't requested.
            if (!scheduled)
            {
                scheduled = true;
                startTime = AudioSettings.dspTime;
            }
            
            var playNext = Sound.HasPlayFlag(playFlags, PlaySoundFlags.PlayNext);
            
            // If we're playing the next child, add the time between to the start time.
            // TODO: Maybe go into a different / "waiting" mode instead, especially if the wait is long?
            if (playNext)
            {
                startTime += _calculators[CoreSchedulerSoundModuleDefinition.TimeBetweenProperty].ValueContainer.FloatValue;
            }
            
            DecrementRepeatsRemaining(_childIndex);
        }

        internal void InitPlaybackValues()
        {
            if (_soundDefinition.SoundType != SoundType.Continuous) return;
            
            _childIndex = -1;
            _repeatsRemaining = _calculators[CoreSchedulerSoundModuleDefinition.RepeatsProperty].ValueContainer.IntValue; 
            _limitedRepeats = (_repeatsRemaining > 0);
            _playMore = true;
        }
        
        protected virtual void DecrementRepeatsRemaining(int childIndexPlayed)
        {
            if (!_limitedRepeats) return;
            
            if (_repeatsRemaining <= 0) return;

            if (ModuleSoundDefinition.RepeatType == ParentSoundRepeatType.Child)
            {
                --_repeatsRemaining;
            }
            else if (ModuleSoundDefinition.RepeatType == ParentSoundRepeatType.Self &&
                     childIndexPlayed >= _soundDefinition.ChildCount - 1)
            {
                // TODO: This seems like it might cause a bug with Random containers?
                --_repeatsRemaining;
            }

            // Debug.Log($"{this} new repeats remaining: {_repeatsRemaining}");
            
            if (_repeatsRemaining <= 0)
            {
                // Debug.Log($"{this} Setting playMore to FALSE");
                _playMore = false;
            }
            
            // Debug.Log($"{this} DECREMENTED repeats {_repeatsRemaining} play more? {_playMore} limited repeats? {_limitedRepeats}");
        }
        
        // TODO: Not used for now, but was used in Audiobread 1.0 by the music containers.
        internal void SetRepeats(int newRepeats)
        {
            _repeatsRemaining = newRepeats;
            if (_repeatsRemaining < 0) // TODO: Shouldn't this be <= 0?.
            {
                _limitedRepeats = false;
                _playMore = true;
            }
            else
            {
                _limitedRepeats = true;
                _playMore = _repeatsRemaining > 0;
            }
            // Debug.Log($"{this} new repeats {_repeatsRemaining} play more? {_playMore} limited repeats? {_limitedRepeats}");
        }
    }
}