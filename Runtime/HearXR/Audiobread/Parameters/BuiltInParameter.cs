using UnityEngine;

namespace HearXR.Audiobread
{
    public abstract class BuiltInParameter : Parameter
    {
        private static Transform _listenerTransform;
        private static bool _listenerTransformInit;
        
        protected static Transform ListenerTransform
        {
            get
            {
                if (!_listenerTransformInit)
                {
                    var listener = FindObjectOfType<AudioListener>();
                    if (listener != null)
                    {
                        _listenerTransform = listener.transform;
                        _listenerTransformInit = true;
                    }
                    else
                    {
                        Debug.LogWarning("Audio listener not found in the scene");
                        _listenerTransform = Camera.main.transform;
                        _listenerTransformInit = true;
                    }
                }

                return _listenerTransform;
            }
        }
        
        public abstract float Calculate(ISound sound, SetValuesType setValuesType);
    }
}