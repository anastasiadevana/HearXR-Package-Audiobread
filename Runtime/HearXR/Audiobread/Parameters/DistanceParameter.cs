// using UnityEngine;

using UnityEngine;

namespace HearXR.Audiobread
{
    // [CreateAssetMenu (fileName = "BuiltInSoundEvent", menuName = "Audiobread/Built In/Parameter/Distance")]
    public class DistanceParameter : BuiltInParameter
    {
        public override float Calculate(ISound sound, SetValuesType setValuesType)
        {
            // Always calculate distance, so we won't check the setValuesType.

            var soundSourceObject = sound.SoundSourceObject;
            
            // TODO: Cache Game Object distance values in some central place.
            
            var distance = Vector3.Distance(ListenerTransform.position, soundSourceObject.transform.position);
            return distance;
        }
    }
}