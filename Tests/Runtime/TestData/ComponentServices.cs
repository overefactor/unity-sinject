using System.Collections.Generic;
using Overefactor.DI.Runtime.Attributes;
using Overefactor.DI.Runtime.Interfaces;
using UnityEngine;

namespace Overefactor.DI.Tests.Tests.Runtime.TestData
{
    [SRegister(typeof(IServiceA))]
    internal class ComponentServiceA : MonoBehaviour, IServiceA
    {
        
    }
    
    [SRegister(typeof(IServiceB))]
    internal class ComponentServiceB : MonoBehaviour, IServiceB
    {
        
    }
    
    internal class ComponentWithDependencyToA : MonoBehaviour
    {
        [SInject] public IServiceA ServiceA;
    }
    
    internal class ComponentWithEvents : MonoBehaviour, ISInjectorRegisterHandler, ISInjectorInjectHandler
    {
        public List<string> Events { get; } = new();
        
        public void OnRegister(ISInjector injector)
        {
            Events.Add("OnRegister");
        }
        
        public void OnInject(ISInjector injector)
        {
            Events.Add("OnInject");
        }
    }
}