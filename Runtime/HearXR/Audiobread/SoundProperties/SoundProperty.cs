using System;
using HearXR.Audiobread.SoundProperties;
using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    public abstract class SoundProperty : ScriptableObject
    {
        public abstract string ShortName { get; }

        public abstract bool Randomizable { get; }
        
        public abstract bool RandomizeOnSoundPlay { get; }
    
        public abstract bool RandomizeOnChildPlay { get; }
        
        public abstract bool ContinuousUpdate { get; }
        
        public abstract bool HasMinLimit { get; }
        
        public abstract bool HasMaxLimit { get; }

        public abstract Calculator CreateCalculator();
        
        public abstract Calculator.CalculationMethod CalculationMethod { get; }
        
        public abstract bool InfluenceChildNodes { get; }
    }
    
    public abstract class SoundProperty<T> : SoundProperty
    {
        public abstract T DefaultValue { get; }

        public abstract T MinLimit { get; }

        public abstract T MaxLimit { get; }

        public abstract ValueContainer<T> CreateValueContainer();
    }
}