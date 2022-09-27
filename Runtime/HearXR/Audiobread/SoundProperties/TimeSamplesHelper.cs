using UnityEngine;

// TODO: Take pitch into account.
namespace HearXR.Audiobread.SoundProperties
{
    public static class TimeSamplesHelper
    {
        public static int ValidateAudioClipOffset(in AudioClip clip, float value)
        {
            int timeSamples = 0;
            
            double offsetInSeconds = 1.0d * value;
            if (offsetInSeconds > 0.0d)
            {
                // Check to make sure that the offset is not longer than the length of the clip.
                var sampleOffset = TimeToSamples(offsetInSeconds, clip.frequency);
                if (sampleOffset < clip.samples)
                {
                    timeSamples = sampleOffset;
                }
                else
                {
                    Debug.LogWarning($"HEAR XR: Offset {offsetInSeconds} is longer than the duration of the clip {clip.length}.");
                }
            }

            return timeSamples;
        }
        
        public static double SamplesToTime(int samples, double singleSampleDuration)
        {
            return samples * singleSampleDuration;
        }

        public static double SamplesToTime(int samples, int frequency)
        {
            double singleSampleDuration = GetSingleSampleDuration(frequency);
            return samples * singleSampleDuration;
        }

        public static int TimeToSamples(double time, int clipFrequency)
        {
            return (int) (clipFrequency * time);
        }

        public static double GetSingleSampleDuration(int clipFrequency)
        {
            return 1.0d / clipFrequency;
        }
    }
}

