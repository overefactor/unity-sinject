using Overefactor.DI.Runtime.Attributes;
using UnityEngine;

namespace Overefactor.DI.Samples.ScriptableObjectService
{
    [SRegister(typeof(ServiceC))]
    [CreateAssetMenu(menuName = "Sapo/DI/Samples/Scriptable Object Service/New Service C")]
    public class ServiceC : ScriptableObject
    {
        [SInject] private IServiceA _serviceA;
        [SInject] private ServiceB _serviceB;
        
        public void Introduce()
        {
            Debug.Log("[ServiceC] Hello, I am ServiceC! I am referencing two services listed below:");
            _serviceA.Introduce();
            _serviceB.Introduce();
        }
    }
}
