using Overefactor.DI.Runtime.Attributes;
using UnityEngine;

namespace Overefactor.DI.Samples.ScriptableObjectService
{
    public class Player : MonoBehaviour
    {
        [SInject] private ServiceC _serviceC;

        private void Awake()
        {
            Debug.Log("[Player] Hello, I am Player! I am referencing service C: ");
            _serviceC.Introduce();
        }
    }   
}