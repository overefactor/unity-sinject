using System;
using System.Collections.Generic;
using Sapo.DI.Runtime.Attributes;

namespace Sapo.DI.Runtime.Interfaces
{
    /// <summary>
    /// Represents an interface for dependency injection, providing functionality
    /// to register, resolve, unregister, and inject dependencies.
    /// </summary>
    public interface ISInjector
    {
        //public ISInjector Parent { get; }
        
        /// <summary>
        /// Tries to resolve the instance of the specified type.
        /// </summary>
        /// <param name="type">The type of the instance to resolve.</param>
        /// <param name="instance">The resolved instance of the specified type if the operation is successful; otherwise, null.</param>
        /// <returns>true if the operation is successful; otherwise, false.</returns>
        public bool TryResolve(Type type, out object instance);


        /// <summary>
        /// Resolves all instances of the specified type.
        /// </summary>
        /// <param name="type">The type of the instances to resolve.</param>
        /// <param name="instances">A list to store the resolved instances.</param>
        public void ResolveAll(Type type, List<object> instances);
        
        /// <summary>
        /// Tries to register an instance of the specified type.
        /// </summary>
        /// <param name="type">The type of the instance to register.</param>
        /// <param name="instance">The instance to register.</param>
        /// <returns>true if the operation is successful; otherwise, false.</returns>
        public bool TryRegister(Type type, object instance);
        
        /// <summary>
        /// Unregisters the specified instance of the specified type.
        /// </summary>
        /// <param name="type">The type of the instance to unregister.</param>
        /// <param name="instance">The instance to unregister.</param>
        public void Unregister(Type type, object instance);

        /// <summary>
        /// Injects dependencies into the specified instance. All fields defined with <see cref="SInjectAttribute"/> will be injected.
        /// </summary>
        /// <param name="instance">The instance to inject.</param>
        public void Inject(object instance);
    }
}
