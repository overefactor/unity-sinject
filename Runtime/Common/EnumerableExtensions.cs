using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sapo.DI.Runtime.Common
{
    internal static class EnumerableExtensions
    {
        internal static bool IsEmpty<T>(this IEnumerable<T> enumerable) => !enumerable.Any();


        internal static Array CastToArray(this IList list, Type elementType)
        {
            var result = Array.CreateInstance(elementType, list.Count);
            list.CopyTo(result, 0);
            return result;
        }

        internal static IList CastToList(this IList list, Type elementType)
        {
            var result = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
            foreach (var o in list) result.Add(o);
            return result;
        }
        
        
        
        
    }
}