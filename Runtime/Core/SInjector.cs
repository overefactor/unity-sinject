using System;
using System.Collections.Generic;
using System.Linq;
using Sapo.DI.Runtime.Common;
using Sapo.DI.Runtime.Interfaces;
using UnityEngine;

namespace Sapo.DI.Runtime.Core
{
    /// <summary>
    /// A simple implementation of <see cref="ISInjector"/> that uses reflection to inject dependencies.
    /// </summary>
    public sealed class SInjector : ISInjector
    {
        private static ISReflectionCache _reflectionCache;
        internal static ISReflectionCache ReflectionCache
        {
            get
            {
                if (_reflectionCache != null) return _reflectionCache;
                
                _reflectionCache = new SReflectionCache();
                _reflectionCache.Build();
                return _reflectionCache;
            }
        }

        private readonly ISInjector _parent;
        private readonly Dictionary<Type, SInstanceCollection> _instances = new();

        internal IEnumerable<object> RegisteredInstances =>
            _instances.Values.SelectMany(c => c.AliveInstances).ToArray();

        public SInjector() => _instances[typeof(ISInjector)] = SInstanceCollection.With(this);

        public SInjector(SInjector parent) : this() => _parent = parent;
        
        private bool TryResolveInSelf(Type type, out object instance)
        {
            if (!_instances.TryGetValue(type, out var collection))
            {
                instance = null;
                return false;
            }

            instance = collection.Primary;
            if (instance != null) return true;

            _instances.Remove(type);
            instance = null;
            return false;
        }
        
        private bool TryResolveCollection(Type type, out object collection)
        {
            var result = new List<object>();

            if (type.IsArray(out var elementType) || type.IsEnumerable(out elementType))
            {
                ResolveAll(elementType, result);
                collection = result.CastToArray(elementType);
                return true;
            }

            if (type.IsList(out elementType))
            {
                ResolveAll(elementType, result);
                collection = result.CastToList(elementType);
                return true;
            }
            
            collection = null;
            return false;
        }
        
        public bool TryResolve(Type type, out object instance)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (TryResolveCollection(type, out instance)) return true;
            
            if (TryResolveInSelf(type, out instance)) return true;
            return _parent?.TryResolve(type, out instance) ?? false;
        }

        public void ResolveAll(Type type, List<object> instances)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            if (_instances.TryGetValue(type, out var collection)) 
                instances.AddRange(collection.AliveInstances);

            _parent?.ResolveAll(type, instances);
        }

        public bool TryRegister(Type type, object instance)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            if (!type.IsInstanceOfType(instance))
                throw new ArgumentException("The instance type must be assignable to the specified type.");

            if (!_instances.TryGetValue(type, out var collection))
            {
                collection = SInstanceCollection.Empty();
                _instances.Add(type, collection);
            }
            
            return collection.TryRegister(instance);
        }
        
        public void Unregister(Type type, object instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            if (!_instances.TryGetValue(type, out var collection)) return;
            
            collection.Unregister(instance);
            if (collection.IsEmpty) _instances.Remove(type);
        }

        public void Inject(object instance)
        {
            TryInjectDependencies(instance);
            TryInjectComponents(instance);
        }

        private void TryInjectDependencies(object instance)
        {
            var type = instance.GetType();
            var fields = ReflectionCache.GetSInjectFields(type);
            if (fields.IsEmpty()) return;

            foreach (var field in fields) field.SetValue(instance, this.Resolve(field.FieldType));
        }
        
        private void TryInjectComponents(object instance)
        {
            var type = instance.GetType();
            if (instance is not Component component) return;
            
            var fields = ReflectionCache.GetCInjectFields(type);
            if (fields.IsEmpty()) return;

            foreach (var field in fields) field.SetValue(instance, component.GetComponent(field.FieldType));
        }

        internal void PerformSelfInjection()
        {
            foreach (var instance in _instances.Values.SelectMany(c => c.Instances)) 
                Inject(instance);
        }
        
        internal void ForceCopyFrom(SInjector injector)
        {
            foreach (var (type, instance) in injector._instances)
                _instances[type] = instance;
        }
    }
}