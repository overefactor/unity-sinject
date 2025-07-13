using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Overefactor.DI.Runtime.Common
{
    internal static class ReflectionExtensions
    {
        internal static bool IsDefinedWithAttribute<T>(this MemberInfo member) where T : Attribute =>
            Attribute.GetCustomAttribute(member, typeof(T)) != null;
        
        internal static bool IsDefinedWithAttribute<T>(this MemberInfo member, out T attribute) where T : Attribute
        {
            attribute = GetDefinedAttribute<T>(member);
            return attribute != null;
        }

        internal static T GetDefinedAttribute<T>(this MemberInfo member) where T : Attribute =>
            (T)Attribute.GetCustomAttribute(member, typeof(T));
        
        internal static bool IsDefinedWithAttribute<T>(this Type type) where T : Attribute =>
            type.GetDefinedAttribute<T>() != null;

        internal static bool IsDefinedWithAttribute<T>(this Type type, out T attribute) where T : Attribute
        {
            attribute = GetDefinedAttribute<T>(type);
            return attribute != null;
        }
        
        internal static T GetDefinedAttribute<T>(this Type type) where T : Attribute
        {
            while (type != null && type != typeof(object))
            {
                var attribute = (T)type.GetCustomAttribute(typeof(T), false);
                if (attribute != null) return attribute;
                type = type.BaseType;
            }

            return null;
        }

        internal static IEnumerable<FieldInfo> GetFieldsWithAttribute<TAttribute>(this Type type) where TAttribute : Attribute
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                                       BindingFlags.FlattenHierarchy;

            return type.GetFields(flags).Where(f => f.IsDefinedWithAttribute<TAttribute>());
        }
        
        internal static IEnumerable<Type> SafelyGetTypes(this Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (Exception e)
            {
                // ignore
            }

            return Type.EmptyTypes;
        }

        internal static bool IsArray(this Type type, out Type elementType)
        {
            if (!type.IsArray || type.GetArrayRank() != 1)
            {
                elementType = null;
                return false;
            }
            
            elementType = type.GetElementType();
            return true;
        }

        internal static bool IsList(this Type type, out Type elementType)
        {
            elementType = null;
            if (!type.IsGenericType) return false;
            if (type.GetGenericTypeDefinition() != typeof(List<>)) return false;

            elementType = type.GetGenericArguments()[0];
            return true;
        }

        internal static bool IsEnumerable(this Type type, out Type elementType)
        {
            elementType = null;
            if (!type.IsGenericType) return false;
            if (type.GetGenericTypeDefinition() != typeof(IEnumerable<>)) return false;
            
            elementType = type.GetGenericArguments()[0];
            return true;
        }
    }
}