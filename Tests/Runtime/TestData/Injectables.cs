using Overefactor.DI.Runtime.Attributes;
using Overefactor.DI.Runtime.Interfaces;

namespace Overefactor.DI.Tests.Tests.Runtime.TestData
{
    internal class PrivateFieldInjectable
    {
        [SInject] private IServiceA _service;
        
        public IServiceA Service => _service;
    }
    
    internal class PublicFieldInjectable
    {
        [SInject] public IServiceA Service;
    }
    
    internal class MultipleFieldsInjectable
    {
        [SInject] public IServiceA ServiceA;
        [SInject] public IServiceB ServiceB;
    }

    internal class NoInjectableFieldsInjectable
    {
        
    }
    
    internal class InjectorFieldInjectable
    {
        [SInject] public ISInjector Injector;
    }
}