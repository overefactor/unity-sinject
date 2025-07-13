using System;

namespace Overefactor.DI.Runtime.Attributes
{
    /// <summary>
    /// The SRegister attribute is used to mark Unity Components and ScriptableObjects that should be registered for dependency injection.
    /// </summary>
    /// <example>
    /// This sample shows how to use the SRegister attribute.
    /// <code>
    /// [SRegister(typeof(IMyInterface))]
    /// public class MyComponent : MonoBehaviour, IMyInterface
    /// {
    ///     // Implementation of IMyInterface
    /// }
    /// 
    /// [SRegister(typeof(IMyScriptableObject))]
    /// public class MyScriptableObject : ScriptableObject, IMyScriptableObject
    /// {
    ///     // Implementation of IMyScriptableObject
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class SRegister : Attribute
    {
        /// <summary>
        /// A type that the Unity Component or ScriptableObject implements.
        /// </summary>
        public Type Type { get; }
        
        public bool RegisterAllInterfaces { get; set; }

        /// <summary>
        /// Constructor for the SRegister attribute.
        /// </summary>
        /// <param name="type">A type that the Component/ScriptableObject implements.</param>
        /// <param name="registerAllInterfaces">If true, all interfaces that the class implements will be registered.</param>
        public SRegister(Type type = null, bool registerAllInterfaces = false)
        {
            Type = type;
            RegisterAllInterfaces = registerAllInterfaces;
        }

        /// <summary>
        /// Constructor for the SRegister attribute.
        /// </summary>
        /// <param name="registerAllInterfaces">If true, all interfaces that the class implements will be registered.</param>
        public SRegister(bool registerAllInterfaces = false)
        {
            RegisterAllInterfaces = registerAllInterfaces;
        }
        
        /// <summary>
        /// Constructor for the SRegister attribute.
        /// </summary>
        public SRegister()
        {
            
        }
    }
}