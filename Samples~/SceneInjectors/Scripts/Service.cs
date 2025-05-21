using Sapo.DI.Runtime.Attributes;
using UnityEngine;

namespace Sapo.DI.Samples.SceneInjectors.PackageSamples.sk.sapo.dependency_injection.SceneInjectors.Scripts
{
    [SRegister(typeof(IService))]
    public class Service : MonoBehaviour, IService
    {
        
    }
}