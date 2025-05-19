using System;
using UnityEngine;

namespace Sapo.DI.Runtime.Interfaces
{
    /// <summary>
    /// Provides extension methods for the <see cref="ISInjector"/> interface to simplify dependency resolution.
    /// </summary>
    public static class SInjectorExtensions
    {
        /// <summary>
        /// Resolves the instance of the specified type.
        /// </summary>
        /// <param name="injector">The instance of the <see cref="ISInjector"/> to use for resolving dependencies.</param>
        /// <typeparam name="T">The type of the instance to resolve.</typeparam>
        /// <returns>The resolved instance of the specified type.</returns>
        public static T Resolve<T>(this ISInjector injector) => (T)injector.Resolve(typeof(T));

        /// <summary>
        /// Resolves the instance of the specified type.
        /// </summary>
        /// <param name="injector">The instance of the <see cref="ISInjector"/> to use for resolving dependencies.</param>
        /// <param name="type">The type of the instance to resolve.</param>
        /// <returns>The resolved instance of the specified type.</returns>
        public static object Resolve(this ISInjector injector, Type type)
        {
            if (injector.TryResolve(type, out var instance)) return instance;

            Debug.LogError($"[Sapo.DI] Unable to resolve <color=#ff8000>{type}</color> type. Make sure it's registered.");
            return null;
        }
        
        /// <summary>
        /// Tries to resolve the instance of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the instance to resolve.</typeparam>
        /// <param name="injector">The instance of the <see cref="ISInjector"/> to use for resolving dependencies.</param>
        /// <param name="instance">The resolved instance of the specified type if the operation is successful; otherwise, default value of T.</param>
        /// <returns>true if the operation is successful; otherwise, false.</returns>
        public static bool TryResolve<T>(this ISInjector injector, out T instance)
        {
            if (injector.TryResolve(typeof(T), out var obj))
            {
                instance = (T)obj;
                return true;
            }

            instance = default;
            return false;
        }
        
        /// <summary>
        /// Checks if the specified type is registered.
        /// </summary>
        /// <typeparam name="T">The type to check.</typeparam>
        /// <param name="injector">The instance of the <see cref="ISInjector"/> to use for resolving dependencies.</param>
        /// <returns>true if the type is registered; otherwise, false.</returns>
        public static bool IsRegistered<T>(this ISInjector injector) => injector.IsRegistered(typeof(T));

        /// <summary>
        /// Checks if the specified type is registered.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="injector">The instance of the <see cref="ISInjector"/> to use for resolving dependencies.</param>
        /// <returns>true if the type is registered; otherwise, false.</returns>
        public static bool IsRegistered(this ISInjector injector, Type type) => injector.TryResolve(type, out _);

        /// <summary>
        /// Registers an instance of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the instance to register.</typeparam>
        /// <param name="injector">The instance of the <see cref="ISInjector"/> to use for resolving dependencies.</param>
        /// <param name="instance">The instance to register.</param>
        public static void Register<T>(this ISInjector injector, object instance) =>
            injector.Register(typeof(T), instance);
        
        /// <summary>
        /// Registers an instance of the specified type.
        /// </summary>
        /// <param name="injector">The instance of the <see cref="ISInjector"/> to use for resolving dependencies.</param>
        /// <param name="type">The type of the instance to register.</param>
        /// <param name="instance">The instance to register.</param>
        public static void Register(this ISInjector injector, Type type, object instance)
        {
            if (injector.TryRegister(type, instance)) return;

            Debug.LogError($"[Sapo.DI] Unable to register <color=#ff8000>{type}</color> type.");
        }

        /// <summary>
        /// Tries to register an instance of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the instance to register.</typeparam>
        /// <param name="injector">The instance of the <see cref="ISInjector"/> to use for resolving dependencies.</param>
        /// <param name="instance">The instance to register.</param>
        /// <returns>true if the operation is successful; otherwise, false.</returns>
        public static bool TryRegister<T>(this ISInjector injector, object instance) =>
            injector.TryRegister(typeof(T), instance);

        /// <summary>
        /// Unregisters the specified instance of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the instance to unregister.</typeparam>
        /// <param name="injector">The instance of the <see cref="ISInjector"/> to use for resolving dependencies.</param>
        /// <param name="instance">The instance to unregister.</param>
        public static void Unregister<T>(this ISInjector injector, object instance) =>
            injector.Unregister(typeof(T), instance);
    }
}