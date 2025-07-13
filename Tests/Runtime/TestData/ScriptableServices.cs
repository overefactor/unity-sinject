using System.Collections.Generic;
using Overefactor.DI.Runtime.Attributes;
using Overefactor.DI.Runtime.Interfaces;
using UnityEngine;

namespace Overefactor.DI.Tests.Tests.Runtime.TestData
{
    [SRegister(typeof(IServiceA))]
    internal class ScriptableServiceA : ScriptableObject, IServiceA
    {
        
    }
    
    internal class ScriptableServiceAInherited : ScriptableServiceA
    {
        
    }
    
    internal class ScriptableServiceAWithEvents : ScriptableServiceA, ISInjectorRegisterHandler, ISInjectorInjectHandler
    {
        public List<string> Events { get; } = new();

        public void OnRegister(ISInjector injector) => Events.Add("OnRegister");

        public void OnInject(ISInjector injector) => Events.Add("OnInject");
    }
    
    [SRegister(typeof(IServiceB))]
    internal class ScriptableServiceBWithBaseServiceA : ScriptableServiceA, IServiceB
    {
        
    }
}