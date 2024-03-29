using System;
using System.Collections.Generic;
using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    public abstract class Calculator
    {
        public bool Active { get; protected set; }
        
        public abstract ValueContainer ValueContainer { get; }

        public abstract void Generate();
        
        protected abstract void Calculate();
        
        public abstract void Calculate(ref Dictionary<Parameter, float> parameterValues);

        public abstract void AddParameterDefinition(SoundParameterDefinition parameter);

        public abstract void SetDefinition(Definition definition);

        public abstract void UnsetDefinition();

        public abstract void AddInfluence(ValueContainer influence);

        public abstract void RemoveInfluence(ValueContainer influence);
    }
    
    public abstract class Calculator<T> : Calculator {}
    
    public abstract class Calculator<T, TProperty, TDefinition> : Calculator<T>
        where TProperty : SoundProperty<T>
        where TDefinition : Definition<T, TProperty>
    {
        #region Properties
        public TProperty Property => _property;
        
        /// <summary>
        /// This is the one you want!
        /// </summary>
        public T Value => _value;
        
        /// <summary>
        /// This is the unaffected value of this sound, before adding parameters, fades, mixer groups, etc.
        /// </summary>
        public T RawValue => _rawValue;  // TODO: Nobody should be hitting this ever (probably)

        public override ValueContainer ValueContainer => _valueContainer;
        
        public ValueContainer<T> TypedValueContainer => _valueContainer;
        #endregion
        
        #region Private Fields
        protected readonly TProperty _property;
        protected TDefinition _definition;
        protected readonly ValueContainer<T> _valueContainer;
        protected T _rawValue;
        protected T _value;
        protected List<SoundParameterDefinition> _parameterDefinitions = new List<SoundParameterDefinition>();
        protected SoundParameterDefinition[] _parameterArray;
        protected List<ValueContainer<T>> _influences = new List<ValueContainer<T>>();
        protected T _randomizedOffset;
        protected T _baseValue;
        protected float _parameterFactor = 1.0f;
        protected float? _parameterAddition = null;
        protected float? _parameterOverride = null;
        #endregion
        
        protected Calculator(TProperty soundProperty)
        {
            _property = soundProperty;
            _valueContainer = _property.CreateValueContainer();
            _parameterArray = new SoundParameterDefinition[0];
        }

        #region Public Methods
        public override void SetDefinition(Definition definition)
        {
            _definition = (TDefinition) definition;
            Active = _definition.active;
            Generate();
            Calculate();
        }

        public override void UnsetDefinition()
        {
            _definition = null;
            _rawValue = _property.DefaultValue;
            _value = _property.DefaultValue;
        }

        public override void AddInfluence(ValueContainer influence)
        {
            var tInfluence = (ValueContainer<T>) influence;
            
            if (_influences.Contains(tInfluence))
            {
                Debug.LogWarning("This value is already affecting this sound property.");
                return;
            }
            _influences.Add(tInfluence);
        }

        public override void RemoveInfluence(ValueContainer influence)
        {
            var tInfluence = (ValueContainer<T>) influence;
            
            if (_influences.Contains(tInfluence))
            {
                _influences.Remove(tInfluence);
            }
        }

        public override void AddParameterDefinition(SoundParameterDefinition parameter)
        {
            if (!_parameterDefinitions.Contains(parameter))
            {
                _parameterDefinitions.Add(parameter);
                _parameterArray = _parameterDefinitions.ToArray();
            }
        }
        #endregion
        
        #region Abstract Methods
        public override void Generate()
        {
            if (!Active) return;
            
            _rawValue = _valueContainer.Value;
        }
        #endregion
    }
}
