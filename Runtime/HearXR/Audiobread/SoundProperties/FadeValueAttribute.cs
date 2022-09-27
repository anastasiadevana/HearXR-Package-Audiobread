using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    public class FadeValuesAttribute : PropertyAttribute
    {
        public readonly System.Type propertyType;
        public Fade.Direction fadeDirection;
    
        public FadeValuesAttribute(System.Type propertyType, Fade.Direction fadeDirection)
        {
            this.propertyType = propertyType;
            this.fadeDirection = fadeDirection;
        }
    }
}