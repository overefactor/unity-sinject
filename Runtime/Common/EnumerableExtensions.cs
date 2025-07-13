using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Overefactor.DI.Runtime.Common
{
    internal static class EnumerableExtensions
    {
        internal static bool IsEmpty<T>(this IEnumerable<T> enumerable) => !enumerable.Any();


        internal static Array CastToArray<T>(this ISet<T> set, Type elementType)
        {
            var result = Array.CreateInstance(elementType, set.Count);

            var i = 0;
            foreach (var instance in set) result.SetValue(instance, i++);
            
            return result;
        }

        internal static IList CastToList<T>(this ISet<T> set, Type elementType)
        {
            var result = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
            foreach (var instance in set) result.Add(instance);
            return result;
        }
        
        
        
        
    }
}