using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    // TODO: Don't even set the FadeIn here. Just pass it in when starting to play sound or stopping sound.
    // TODO: Some of this should probably move to the FloatCalculator.
    public class VolumeCalculator : FloatCalculator
    {
        public VolumeCalculator(FloatSoundProperty soundProperty) : base(soundProperty) {}

        public bool HasFadeOut => _hasFadeOut;

        public void SetFades(Fade fadeIn = null, Fade fadeOut = null)
        {
            _hasFadeIn = false;
            _hasFadeOut = false;
            _fadeIn = null;
            _fadeOut = null;
            
            if (fadeIn != null)
            {
                _hasFadeIn = true;
                _fadeIn = fadeIn;
            }
            
            if (fadeOut != null)
            {
                _hasFadeOut = true;
                _fadeOut = fadeOut;
            }
            
            Generate();
            Calculate();
        }

        #region Events
        // public event Action<Fade.Direction> OnFadeFinished;
        public event Action OnFadeOutFinished;
        public event Action OnFadeInFinished;
        #endregion

        #region Properties
        public Fade.Direction FadeDirection => _fadeDirection;
        #endregion
        
        #region Private Fields
        private Fade.Direction _fadeDirection = Fade.Direction.None;
        private Coroutine _fadeCoroutine;
        private Fade _fadeIn;
        private Fade _fadeOut;
        private bool _hasFadeIn;
        private bool _hasFadeOut;
        
        private float _parameterFactor = 1.0f;
        private float _fadeFactor = 1.0f;
        private float _totalFactor = 1.0f;
        private float _influenceFactor = 1.0f;

        private bool _shouldFadeIn;
        private float _fadeDuration;
        private float _fadeFrom;
        #endregion
        
        #region SoundPropertyController Abstract Methods
        public override void Calculate()
        {
            _influenceFactor = 1.0f;

            for (int i = 0; i < _influences.Count; ++i)
            {
                // Debug.Log($"{i}: {_influences[i].Value}");
                _influenceFactor *= _influences[i].Value;
            }
            
            // Total factor.
            _totalFactor = 1.0f * _parameterFactor * _fadeFactor;

            // Adjusted value.
            _value = _rawValue * _totalFactor * _influenceFactor;
            
            Mathf.Clamp(_value, _property.MinLimit, _property.MaxLimit);
            
            _valueContainer.FloatValue = _value;
        }
        
        public override void Calculate(ref Dictionary<Parameter, float> parameterValues)
        {
            // TODO: Push higher into a parent, since this is not volume-specific.
            // Parameters
            _parameterFactor = 1.0f;
            for (int i = 0; i < _parameterArray.Length; ++i)
            {
                if (!parameterValues.ContainsKey(_parameterArray[i].parameter))
                {
                    // TODO: Barf better.
                    continue;
                }

                _parameterFactor *=
                    _parameterArray[i].GetSoundPropertyValue(parameterValues[_parameterArray[i].parameter]);
            }
            
            Calculate();
        }
        #endregion
        
        #region Internal Methods
        // TODO: Consolidate this with OnStopSound() - there's code duplication.
        internal void StartFadeOut()
        {
            _shouldFadeIn = false;
            if (!_hasFadeIn && !_hasFadeOut)
            {
                return;
            }
            
            var fadeFrom = 1.0f;
            
            if (_fadeDirection == Fade.Direction.In || _fadeDirection == Fade.Direction.Out)
            {
                fadeFrom = _fadeFactor;
                Audiobread.Instance.StopCoroutine(_fadeCoroutine);
                _fadeDirection = Fade.Direction.None;
            }
            
            // If there is no fadeout specified, or instant stop was requested, do not proceed.
            if (_fadeOut == null || _fadeOut.duration <= 0.0f)
            {
                return;
            }

            // If we're already partially faded, don't take the entire time to do it.
            var duration = _fadeOut.duration;
            if (fadeFrom < 1.0f)
            {
                duration *= fadeFrom;
            }

            // TODO: Add ability to preview fades in editor.
            if (Application.isPlaying)
            {
                _fadeCoroutine = Audiobread.Instance.StartCoroutine(DoFade(fadeFrom, 0.0f, duration));   
            }
            _fadeDirection = Fade.Direction.Out;
        }
        
        internal void PrepareToPlay(PlaySoundFlags playFlags = PlaySoundFlags.None)
        {
            _shouldFadeIn = false;
            _fadeDuration = 0.0f;
            _fadeFrom = 0.0f;

            if (!_hasFadeIn && !_hasFadeOut) return;
            
            bool ignoreFadeIn = Sound.HasPlayFlag(playFlags, PlaySoundFlags.Instant);
            
            // TODO: Make sure we can test fades in editor.
            if (!Application.isPlaying)
            {
                ignoreFadeIn = true;
            }
            
            float fadeFrom = 0.0f;
            
            if (_fadeDirection == Fade.Direction.In || _fadeDirection == Fade.Direction.Out)
            {
                fadeFrom = _fadeFactor;
                // TODO: I guess don't do this as a coroutine :/ because we can't test it in editor.
                Audiobread.Instance.StopCoroutine(_fadeCoroutine);
                _fadeDirection = Fade.Direction.None;
                _shouldFadeIn = false;
            }
            
            if (_fadeIn == null || _fadeIn.duration <= 0.0f || ignoreFadeIn)
            {
                return;
            }

            _shouldFadeIn = true;
            
            // If we're already partially faded, don't take the entire time to do it.
            float duration = _fadeIn.duration;
            if (fadeFrom > 0.0f)
            {
                duration *= (1.0f - fadeFrom);
            }

            _fadeDuration = duration;
            _fadeFrom = fadeFrom;
            _fadeFactor = _fadeFrom;
        }

        internal void OnSoundBegan()
        {
            if (!_shouldFadeIn) return;
            
            // // TODO: Preview fades in editor - Looks like maybe I DO need a light editor-friendly singleton of some sort.
            if (Application.isPlaying)
            {
                // TODO: No magic numbers.
                _fadeCoroutine = Audiobread.Instance.StartCoroutine(DoFade(_fadeFrom, 1.0f, _fadeDuration));
            }
            _fadeDirection = Fade.Direction.In;
        }
        
        internal bool OnStopSound(StopSoundFlags stopFlags = StopSoundFlags.None)
        {
            _shouldFadeIn = false;
            if (!_hasFadeIn && !_hasFadeOut)
            {
                return false;
            }

            bool ignoreFadeOut = Sound.HasStopFlag(stopFlags, StopSoundFlags.Instant);
            
            float fadeFrom = 1.0f;
            
            if (_fadeDirection == Fade.Direction.In || _fadeDirection == Fade.Direction.Out)
            {
                fadeFrom = _fadeFactor;
                Audiobread.Instance.StopCoroutine(_fadeCoroutine);
                _fadeDirection = Fade.Direction.None;
            }
            
            // If there is no fadeout specified, or instant stop was requested, do not proceed.
            if (_fadeOut == null || _fadeOut.duration <= 0.0f || ignoreFadeOut)
            {
                return false;
            }

            // If we're already partially faded, don't take the entire time to do it.
            float duration = _fadeOut.duration;
            if (fadeFrom < 1.0f)
            {
                duration *= fadeFrom;
            }

            // TODO: Add ability to preview fades in editor.
            if (Application.isPlaying)
            {
                _fadeCoroutine = Audiobread.Instance.StartCoroutine(DoFade(fadeFrom, 0.0f, duration));   
            }
            _fadeDirection = Fade.Direction.Out;
            return true;
        }
        #endregion
        
        #region Coroutines
        private IEnumerator DoFade(float from, float to, float duration)
        {
            // TODO: Make the fade non-linear.
            // TODO: Handle pauses.
            
            _shouldFadeIn = false;
            
            float currentTime = 0.0f;
            _fadeFactor = from;
            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                _fadeFactor = Mathf.Lerp(from, to, currentTime / duration);
                yield return null;
            }
            _fadeFactor = to;

            if (_fadeDirection == Fade.Direction.In)
            {
                OnFadeInFinished?.Invoke();
            }
            else if (_fadeDirection == Fade.Direction.Out)
            {
                OnFadeOutFinished?.Invoke();
            }

            _fadeDirection = Fade.Direction.None;
        }
        #endregion
    }
}
