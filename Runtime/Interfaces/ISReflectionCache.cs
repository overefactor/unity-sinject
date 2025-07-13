using System;
using System.Reflection;

namespace Overefactor.DI.Runtime.Interfaces
{
    internal interface ISReflectionCache
    {
        (Type componentType, Type[] registerTypes)[] RegistrableComponents { get; }
        
        Type[] InjectableComponents { get; }
        
        void Build();
        
        Type[] GetRegisterTypes(Type type);
        
        FieldInfo[] GetSInjectFields(Type type);

        FieldInfo[] GetCInjectFields(Type type);
    }
}