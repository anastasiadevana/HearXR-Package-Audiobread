using System;
using UnityEngine;

namespace HearXR.Audiobread
{
    public class DistanceParameter : BuiltInParameter
    {
        #region Private Fields
        [NonSerialized] private bool _objectLost;
        [NonSerialized] private float _lastDistance;
        #endregion

        
        public override float Calculate(ISound sound, SetValuesType setValuesType)
        {
            // Always calculate distance, so we won't check the setValuesType.
            var soundSourceObject = sound.SoundSourceObject;
            
            if (_objectLost) return _lastDistance;

            // TODO: Cache Game Object distance values in some central place.
            
            try
            {
                _lastDistance = Vector3.Distance(ListenerTransform.position, soundSourceObject.transform.position);  
            }
            catch (Exception e)
            {
                _objectLost = true;
            }

            return _lastDistance;

        }
    }
}