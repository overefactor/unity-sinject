using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Overefactor.DI.Runtime.Common
{
    internal class SFieldReflectionCache<TAttribute> where TAttribute : Attribute
    {
        private readonly Dictionary<Type, FieldInfo[]> _cache = new();


        
        public bool Build(Type type)
        {
            var fields = ReflectInjectFields(type).ToArray();
            if (fields.IsEmpty()) return false;
            
            _cache[type] = fields;
            return true;
        }
        
        private IEnumerable<FieldInfo> ReflectInjectFields(Type type)
        {
            var fields = type.GetFieldsWithAttribute<TAttribute>();
            
            var baseType = type.BaseType;
            if (CannotReflect(baseType)) return fields;
            
            return fields.Concat(ReflectInjectFields(baseType));
        }
        
        private bool CannotReflect(Type type) => type == null || type == typeof(object) || type == typeof(Object);



        public FieldInfo[] GetFields(Type type) =>
            _cache.TryGetValue(type, out var fields) ? fields : Array.Empty<FieldInfo>();
    }
}