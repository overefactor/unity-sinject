using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Overefactor.DI.Runtime.Attributes;
using Overefactor.DI.Runtime.Interfaces;
using UnityEngine;

namespace Overefactor.DI.Runtime.Common
{
    internal class SReflectionCache : ISReflectionCache
    {
        private readonly Dictionary<Type, Type[]> _registrableTypesCache = new();
        
        public (Type componentType, Type[] registerTypes)[] RegistrableComponents { get; private set; }
        
        public Type[] InjectableComponents { get; private set; }

        private readonly SFieldReflectionCache<SInjectAttribute> _sInjectFieldsCache = new();
        private readonly SFieldReflectionCache<CInjectAttribute> _cInjectFieldsCache = new();

        public void Build()
        {
            var components = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.SafelyGetTypes());
            
            var registrableComponents = new List<(Type componentType, Type[] registerTypes)>();
            var injectableComponents = new List<Type>();
            var componentType = typeof(Component);
            
            foreach (var type in components)
            {
                var isComponent = componentType.IsAssignableFrom(type);
                var isRegistrable = type.IsDefinedWithAttribute<SRegister>();
                
                if (isRegistrable) _registrableTypesCache[type] = ReflectRegisterTypes(type).ToArray();
                if (isRegistrable && isComponent) registrableComponents.Add((type, GetRegisterTypes(type)));

                var injectable = _sInjectFieldsCache.Build(type) || _cInjectFieldsCache.Build(type);
                if (injectable && isComponent) injectableComponents.Add(type);
            }
            
            RegistrableComponents = registrableComponents.ToArray();
            InjectableComponents = injectableComponents.ToArray();
        }

        public Type[] GetRegisterTypes(Type type) =>
            _registrableTypesCache.GetValueOrDefault(type, Array.Empty<Type>());

        private IEnumerable<Type> ReflectRegisterTypes(Type type)
        {
            var result = Enumerable.Empty<Type>();
            
            foreach (var attribute in type.GetCustomAttributes<SRegister>())
            {
                if (!attribute.RegisterAllInterfaces)
                {
                    result = result.Append(attribute.Type ?? type);
                    continue;
                }

                if (attribute.Type != null) result = result.Append(attribute.Type);
                result = result.Concat(type.GetInterfaces());
            }
            
            return result.Distinct();
        }

        public FieldInfo[] GetSInjectFields(Type type) => _sInjectFieldsCache.GetFields(type);
        
        public FieldInfo[] GetCInjectFields(Type type) => _cInjectFieldsCache.GetFields(type);
    }
}