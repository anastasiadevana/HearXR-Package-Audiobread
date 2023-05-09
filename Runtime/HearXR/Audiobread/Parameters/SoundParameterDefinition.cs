using UnityEngine;
using HearXR.Audiobread.SoundProperties;

namespace HearXR.Audiobread
{
    /// <summary>
    /// Sound parameter is saved with the sound definition.
    /// It describes the relationship between a specific parameter and a sound property based on a curve.
    /// </summary>
    [CreateAssetMenu(fileName = "~~NameThis~~SoundParameterDefinition~PlaceInResourcesFolder", menuName = "Audiobread/Sound Parameter Definition", order = 200)]
    public class SoundParameterDefinition : ScriptableObject
    {
        public Parameter parameter;
        public SoundProperty soundProperty;
        public AnimationCurve curve;
        public CalculationMethod calculationMethod = CalculationMethod.Multiplication;

        // TODO: Make curves Scriptable Objects, so that they can be reused.

        // TODO: Parameter cannot guarantee to return a float, in case we have INT sound properties.
        // TODO: Property itself must take input from parameter.
        public float GetSoundPropertyValue(float parameterValue)
        {
            return curve.Evaluate(parameterValue);
        }
        
        public Parameter Parameter
        {
            get => parameter;
            set => parameter = value;
        }

        public SoundProperty SoundProperty
        {
            get => soundProperty;
            set => soundProperty = value;
        }

        public AnimationCurve Curve
        {
            get => curve;
            set => curve = value;
        }

        public CalculationMethod CalculationMethod
        {
            get => calculationMethod;
            set => calculationMethod = value;
        }
    }   
}