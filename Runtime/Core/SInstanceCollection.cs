using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace Sapo.DI.Runtime.Core
{
    internal class SInstanceCollection
    {
        private readonly Dictionary<object, LinkedListNode<object>> _instanceNodes = new();
        private readonly LinkedList<object> _instances = new();

        public object Primary => AliveInstances.FirstOrDefault();

        public IEnumerable<object> Instances
        {
            get
            {
                foreach (var instance in _instances)
                    yield return instance;
            }
        }

        public IEnumerable<object> AliveInstances
        {
            get
            {
                var current = _instances.First;
                
                while (current != null)
                {
                    var next = current.Next;
                    
                    if (IsAlive(current.Value)) yield return current.Value;
                    else
                    {
                        _instanceNodes.Remove(current);
                        _instances.Remove(current);
                    }

                    current = next;
                }
            }
        }

        public bool IsEmpty => _instances.Count == 0;

        private SInstanceCollection() { }

        public static SInstanceCollection Empty() => new SInstanceCollection();
        
        public static SInstanceCollection With(object instance)
        {
            var collection = new SInstanceCollection();
            collection.TryRegister(instance);
            return collection;
        }

        private static bool IsAlive(object instance)
        {
            var isObjectOrActiveUnityObject = instance != null && (instance is not Object o || o);
            return isObjectOrActiveUnityObject;
        }
        
        
        public bool TryRegister(object instance)
        {
            if (!IsAlive(instance)) return false;
            if (_instanceNodes.ContainsKey(instance)) return false;

            _instanceNodes.Add(instance, _instances.AddLast(instance));
            return true;
        }
        
        public void Unregister(object instance)
        {
            if (!_instanceNodes.Remove(instance, out var node)) return;
            
            _instances.Remove(node);
        }
        
    }
}