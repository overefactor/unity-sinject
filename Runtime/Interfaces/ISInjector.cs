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
        /// Tries to resolve an instance or collection of the specified type.
        /// </summary>
        /// <param name="type">The type of the instance to resolve.</param>
        /// <param name="instance">The resolved instance of the specified type if the operation is successful; otherwise, null.</param>
        /// <returns>true if the operation is successful; otherwise, false.</returns>
        /// <remarks>
        /// This method supports resolving both single instances and collections. For collections, it supports the following types:
        ///     <c>T[]></c>, <c>IEnumerable&lt;T&gt;</c>, <c>List&lt;T&gt;</c>
        /// <br/>
        /// Examples:
        /// <code>
        /// // Resolve single instance
        /// if (injector.TryResolve(typeof(ILogger), out var logger)) {
        ///     // Use logger instance
        /// }
        /// 
        /// // Resolve collection
        /// if (injector.TryResolve(typeof(IHandler[]), out var handlers)) {
        ///     // handlers contain an array of all registered IHandler instances
        /// }
        /// </code>
        /// </remarks>
        public bool TryResolve(Type type, out object instance);


        /// <summary>
        /// Resolves and appends all instances of the specified type from this injector and all parent injectors.
        /// The instances will be added to the provided list without clearing it first.
        /// </summary>
        /// <param name="type">The type of the instances to resolve.</param>
        /// <param name="instances">A list where resolved instances will be appended.</param>
        public void ResolveAll(Type type, ISet<object> instances);
        
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
