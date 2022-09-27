using UnityEngine;
using HearXR.Audiobread.SoundProperties;

namespace HearXR.Audiobread
{
    /// <summary>
    /// Sound parameter is saved with the sound definition.
    /// It describes the relationship between a specific parameter and a sound property based on a curve.
    /// </summary>
    [System.Serializable]
    public class SoundParameter
    {
        // TODO: Maybe this thing should be called SoundParameterDefinition?
        
        public Parameter parameter;
        public SoundProperty soundProperty;
        public AnimationCurve curve;

        // TODO: Make curves Scriptable Objects, so that they can be reused.

        // TODO: Parameter cannot guarantee to return a float, in case we have INT sound properties.
        // TODO: Property itself must take input from parameter.
        public float GetSoundPropertyValue(float parameterValue)
        {
            return curve.Evaluate(parameterValue);
        }
    }   
}