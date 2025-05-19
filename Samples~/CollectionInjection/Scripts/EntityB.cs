using Sapo.DI.Runtime.Attributes;
using UnityEngine;

namespace Sapo.DI.Samples.CollectionInjection
{
    [CreateAssetMenu(menuName = "Sapo/DI/Samples/Collection Injection/New Entity B")]
    [SRegister(typeof(IEntity))]
    public class EntityB : ScriptableObject, IEntity
    {
        [SerializeField] private string entityName;

        public void Introduce()
        {
            Debug.Log($"Hello I am {entityName}! I am Scriptable Object.");
        }
    }
}