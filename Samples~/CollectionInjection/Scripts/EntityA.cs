using Overefactor.DI.Runtime.Attributes;
using UnityEngine;

namespace Overefactor.DI.Samples.CollectionInjection
{
    [SRegister(typeof(IEntity))]
    public class EntityA : MonoBehaviour, IEntity
    {
        [SerializeField] private string entityName;
        
        public void Introduce()
        {
            Debug.Log($"Hello I am {entityName}! I am Mono Behaviour.");
        }
    }
}