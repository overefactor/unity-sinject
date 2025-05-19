
using UnityEngine;

namespace Sapo.DI.Runtime.Common
{
    internal static class ObjectExtensions
    {
        public static bool IsAlive(this object instance)
        {
            var isObjectOrActiveUnityObject = instance != null && (instance is not Object o || o);
            return isObjectOrActiveUnityObject;
        }
    }
}