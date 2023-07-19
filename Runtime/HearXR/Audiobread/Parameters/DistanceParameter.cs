using System;
using UnityEngine;

namespace HearXR.Audiobread
{
    public class DistanceParameter : BuiltInParameter
    {
        #region Private Fields
        [NonSerialized] private bool _objectLost;
        [NonSerialized] private float _lastDistance;
        [NonSerialized] private int _lastSoundSourceObjectID;
        #endregion

        
        public override float Calculate(ISound sound, SetValuesType setValuesType)
        {
            // Always calculate distance, so we won't check the setValuesType.
            // Sometimes the game object gets destroyed and trying to access it will create an error.
            // In that case, return the last known distance.
            // But don't do that if we have a new sound source object.
            GameObject soundSourceObject;
            try
            {
                soundSourceObject = sound.SoundSourceObject;
                var soundSourceObjectID = soundSourceObject.GetInstanceID();
                if (soundSourceObjectID != _lastSoundSourceObjectID)
                {
                    _lastSoundSourceObjectID = soundSourceObjectID;
                    _objectLost = false;
                }
            }
            catch (Exception e)
            {
                // This means that the object does not exist.
                return _lastDistance;
            }
            
            if (_objectLost) return _lastDistance;

            // TODO: Cache Game Object distance values in some central place.
            // TODO: Get rid of the try / catch statements and see why we may be trying to calculate the distance
            //       parameter a bunch when the sound should be stopped.
            
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