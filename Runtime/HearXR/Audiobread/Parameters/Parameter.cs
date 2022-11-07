using UnityEngine;

namespace HearXR.Audiobread
{
    // TODO: Have built-in parameters, like distance and angle that get set automagically.

    /// <summary>
    /// Describes a parameter and its behavior.
    /// </summary>
    [CreateAssetMenu(fileName = "~~NameThis~~Parameter~PlaceInResourcesFolder", menuName = "Audiobread/Parameter", order = 200)]
    public class Parameter : ScriptableObject
    {
        public float minValue;
        public float maxValue;
        public float defaultValue;

        // TODO: Add easing.
    }
}