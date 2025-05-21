using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Sapo.DI.Runtime.Core;
using Sapo.DI.Runtime.Interfaces;
using Sapo.DI.Tests.Runtime.TestData;
using UnityEngine;
using UnityEngine.TestTools;

namespace Sapo.DI.Tests.Runtime.Core
{
    [TestFixture]
    internal class SInjectorTests
    {
        #region Ctor
        
        [Test]
        public void Ctor_ShouldNotThrowException()
        {
            // Arrange
            
            // Act
            var sut = new SInjector();
            
            // Assert
            Assert.That(sut, Is.Not.Null);
        }
        
        [Test]
        public void Ctor_WithParent_ShouldNotThrowException()
        {
            // Arrange
            var parent = new SInjector();
            
            // Act
            var sut = new SInjector(parent);
            
            // Assert
            Assert.That(sut, Is.Not.Null);
        }
        
        [Test]
        public void Ctor_WithNullParent_ShouldNotThrowException()
        {
            // Arrange
            
            // Act
            var sut = new SInjector(null);
            
            // Assert
            Assert.That(sut, Is.Not.Null);
        }
        
        #endregion

        #region Register<T>
        
        [Test]
        public void GenericRegister_ShouldRegister()
        {
            // Arrange
            var injector = new SInjector();
            var service = new ServiceA();
            
            // Act
            injector.Register<IServiceA>(service);
            
            // Assert
            Assert.That(injector.IsRegistered<IServiceA>(), Is.True);
            Assert.That(injector.Resolve<IServiceA>(), Is.EqualTo(service));
        }
        
        [Test]
        public void GenericRegister_WithNullInstance_ShouldThrowException()
        {
            // Arrange
            var injector = new SInjector();
            
            // Act
            void Act() => injector.Register<IServiceA>(null);
            
            // Assert
            Assert.That(Act, Throws.ArgumentNullException);
            Assert.That(injector.IsRegistered<IServiceA>(), Is.False);
        }

        [Test]
        public void GenericRegister_WithAlreadyRegistered_ShouldRegisterSecondInstance()
        {
            // Arrange
            LogAssert.ignoreFailingMessages = true;
            var injector = new SInjector();
            var service = new ServiceA();
            var service2 = new ServiceA();
            injector.Register<IServiceA>(service);
            
            // Act
            injector.Register<IServiceA>(service2);
            
            // Assert
            Assert.That(injector.IsRegistered<IServiceA>(), Is.True);
            Assert.That(injector.Resolve<IServiceA>(), Is.EqualTo(service));
            Assert.That(injector.RegisteredInstances, Does.Contain(service));
            Assert.That(injector.RegisteredInstances, Does.Contain(service2));
        }

        [Test]
        public void GenericRegister_WithDifferentType_ShouldThrowException()
        {
            // Arrange
            var injector = new SInjector();
            var service = new ServiceA();
            
            // Act
            void Act() => injector.Register<IServiceB>(service);
            
            // Assert
            Assert.That(Act, Throws.ArgumentException);
            Assert.That(injector.IsRegistered<IServiceA>(), Is.False);
            Assert.That(injector.IsRegistered<IServiceB>(), Is.False);
            Assert.That(injector.IsRegistered<ServiceA>(), Is.False);
        }

        [Test]
        public void GenericRegister_WithAlreadyRegisteredAndThenDestroyedInstance_ShouldRegister()
        {
            // Arrange
            var injector = new SInjector();
            var service = ScriptableObject.CreateInstance<ScriptableServiceA>();
            var service2 = ScriptableObject.CreateInstance<ScriptableServiceA>();
            injector.Register<IServiceA>(service);
            
            Object.DestroyImmediate(service);

            // Act
            injector.Register<IServiceA>(service2);
            
            // Assert
            Assert.That(injector.IsRegistered<IServiceA>(), Is.True);
            Assert.That(injector.Resolve<IServiceA>(), Is.EqualTo(service2));
        }

        [Test]
        public void GenericRegister_WithDestroyedInstance_ShouldNotRegister()
        {
            // Arrange
            LogAssert.ignoreFailingMessages = true;
            var injector = new SInjector();
            var service = ScriptableObject.CreateInstance<ScriptableServiceA>();
            Object.DestroyImmediate(service);
            
            // Act
            injector.Register<IServiceA>(service);
            
            // Assert
            Assert.That(injector.IsRegistered<IServiceA>(), Is.False);
            Assert.That(injector.RegisteredInstances, Does.Not.Contains(service));
        }
        
        
        #endregion

        #region Register
        
        [Test]
        public void Register_ShouldRegister()
        {
            // Arrange
            var injector = new SInjector();
            var service = new ServiceA();
            
            // Act
            injector.Register(typeof(IServiceA), service);
            
            // Assert
            Assert.That(injector.IsRegistered<IServiceA>(), Is.True);
            Assert.That(injector.Resolve<IServiceA>(), Is.EqualTo(service));
        }
        
        [Test]
        public void Register_WithNullInstance_ShouldThrowException()
        {
            // Arrange
            var injector = new SInjector();
            
            // Act
            void Act() => injector.Register(typeof(IServiceA), null);
            
            // Assert
            Assert.That(Act, Throws.ArgumentNullException);
            Assert.That(injector.IsRegistered<IServiceA>(), Is.False);
        }
        
        [Test]
        public void Register_WithNullType_ShouldThrowException()
        {
            // Arrange
            var injector = new SInjector();
            var service = new ServiceA();
            
            // Act
            void Act() => injector.Register(null, service);
            
            // Assert
            Assert.That(Act, Throws.ArgumentNullException);
            Assert.That(injector.IsRegistered<ServiceA>(), Is.False);
        }
        
        [Test]
        public void Register_WithAlreadyRegistered_ShouldRegisterSecondInstance()
        {
            // Arrange
            LogAssert.ignoreFailingMessages = true;
            var injector = new SInjector();
            var service = new ServiceA();
            var service2 = new ServiceA();
            injector.Register(typeof(IServiceA), service);
            
            // Act
            injector.Register(typeof(IServiceA), service2);
            
            // Assert
            Assert.That(injector.IsRegistered<IServiceA>(), Is.True);
            Assert.That(injector.Resolve<IServiceA>(), Is.EqualTo(service));
            Assert.That(injector.RegisteredInstances, Does.Contain(service));
            Assert.That(injector.RegisteredInstances, Does.Contain(service2));
        }
        
        [Test]
        public void Register_WithDifferentType_ShouldThrowException()
        {
            // Arrange
            var injector = new SInjector();
            var service = new ServiceA();
            
            // Act
            void Act() => injector.Register(typeof(IServiceB), service);
            
            // Assert
            Assert.That(Act, Throws.ArgumentException);
            Assert.That(injector.IsRegistered<IServiceA>(), Is.False);
            Assert.That(injector.IsRegistered<IServiceB>(), Is.False);
            Assert.That(injector.IsRegistered<ServiceA>(), Is.False);
        }
        
        [Test]
        public void Register_WithAlreadyRegisteredAndThenDestroyedInstance_ShouldRegister()
        {
            // Arrange
            var injector = new SInjector();
            var service = ScriptableObject.CreateInstance<ScriptableServiceA>();
            var service2 = ScriptableObject.CreateInstance<ScriptableServiceA>();
            injector.Register(typeof(IServiceA), service);
            
            Object.DestroyImmediate(service);

            // Act
            injector.Register(typeof(IServiceA), service2);
            
            // Assert
            Assert.That(injector.IsRegistered<IServiceA>(), Is.True);
            Assert.That(injector.Resolve<IServiceA>(), Is.EqualTo(service2));
        }
        
        [Test]
        public void Register_WithDestroyedInstance_ShouldNotRegister()
        {
            // Arrange
            LogAssert.ignoreFailingMessages = true;
            var injector = new SInjector();
            var service = ScriptableObject.CreateInstance<ScriptableServiceA>();
            Object.DestroyImmediate(service);
            
            // Act
            injector.Register(typeof(IServiceA), service);
            
            // Assert
            Assert.That(injector.IsRegistered<IServiceA>(), Is.False);
            Assert.That(injector.RegisteredInstances, Does.Not.Contains(service));
        }
        
        #endregion

        #region TryRegister<T>
        
        [Test]
        public void GenericTryRegister_ShouldRegisterAndReturnTrue()
        {
            // Arrange
            var injector = new SInjector();
            var service = new ServiceA();
            
            // Act
            var result = injector.TryRegister<IServiceA>(service);
            
            // Assert
            Assert.That(result, Is.True);
            Assert.That(injector.IsRegistered<IServiceA>(), Is.True);
            Assert.That(injector.Resolve<IServiceA>(), Is.EqualTo(service));
        }
        
        [Test]
        public void GenericTryRegister_WithNullInstance_ShouldThrowException()
        {
            // Arrange
            var injector = new SInjector();
            
            // Act
            void Act() => injector.TryRegister<IServiceA>(null);
            
            // Assert
            Assert.That(Act, Throws.ArgumentNullException);
            Assert.That(injector.IsRegistered<IServiceA>(), Is.False);
        }
        
        [Test]
        public void GenericTryRegister_WithAlreadyRegistered_ShouldRegisterSecondInstance()
        {
            // Arrange
            var injector = new SInjector();
            var service = new ServiceA();
            var service2 = new ServiceA();
            injector.Register<IServiceA>(service);
            
            // Act
            var result = injector.TryRegister<IServiceA>(service2);
            
            // Assert
            Assert.That(result, Is.True);
            Assert.That(injector.IsRegistered<IServiceA>(), Is.True);
            Assert.That(injector.Resolve<IServiceA>(), Is.EqualTo(service));
            Assert.That(injector.RegisteredInstances, Does.Contain(service));
            Assert.That(injector.RegisteredInstances, Does.Contain(service2));
        }
        
        [Test]
        public void GenericTryRegister_WithDifferentType_ShouldThrowException()
        {
            // Arrange
            var injector = new SInjector();
            var service = new ServiceA();
            
            // Act
            void Act() => injector.TryRegister<IServiceB>(service);
            
            // Assert
            Assert.That(Act, Throws.ArgumentException);
            Assert.That(injector.IsRegistered<IServiceA>(), Is.False);
            Assert.That(injector.IsRegistered<IServiceB>(), Is.False);
            Assert.That(injector.IsRegistered<ServiceA>(), Is.False);
        }
        
        [Test]
        public void GenericTryRegister_WithAlreadyRegisteredAndThenDestroyedInstance_ShouldRegisterAndReturnTrue()
        {
            // Arrange
            var injector = new SInjector();
            var service = ScriptableObject.CreateInstance<ScriptableServiceA>();
            var service2 = ScriptableObject.CreateInstance<ScriptableServiceA>();
            injector.Register<IServiceA>(service);
            
            Object.DestroyImmediate(service);

            // Act
            var result = injector.TryRegister<IServiceA>(service2);
            
            // Assert
            Assert.That(result, Is.True);
            Assert.That(injector.IsRegistered<IServiceA>(), Is.True);
            Assert.That(injector.Resolve<IServiceA>(), Is.EqualTo(service2));
        }
        
        [Test]
        public void GenericTryRegister_WithDestroyedInstance_ShouldNotRegisterAndReturnFalse()
        {
            // Arrange
            var injector = new SInjector();
            var service = ScriptableObject.CreateInstance<ScriptableServiceA>();
            Object.DestroyImmediate(service);
            
            // Act
            var result = injector.TryRegister<IServiceA>(service);
            
            // Assert
            Assert.That(result, Is.False);
            Assert.That(injector.IsRegistered<IServiceA>(), Is.False);
        }
        
        [Test]
        public void TryRegister_RegistersOnlyOneInstance()
        {
            // Arrange
            var injector = new SInjector();
            var service = new ServiceA();
            
            // Act
            var result = injector.TryRegister(typeof(IServiceA), service);
            
            // Assert
            Assert.That(result, Is.True);
            Assert.That(injector.IsRegistered<IServiceA>(), Is.True);
            Assert.That(injector.RegisteredInstances.Count(i => i == service), Is.EqualTo(1));
        }

        [Test]
        public void TryRegister_WithRegisteredSameInstance_ShouldRemainSameAndReturnFalse()
        {
            // Arrange
            var injector = new SInjector();
            var service = new ServiceA();
            injector.Register<IServiceA>(service);
            
            // Act
            var result = injector.TryRegister(typeof(IServiceA), service);
            
            // Assert
            Assert.That(result, Is.False);
            Assert.That(injector.IsRegistered<IServiceA>(), Is.True);
            Assert.That(injector.RegisteredInstances.Count(i => i == service), Is.EqualTo(1));
        }

        
        #endregion

        #region TryRegister

        [Test]
        public void TryRegister_ShouldRegisterAndReturnTrue()
        {
            // Arrange
            var injector = new SInjector();
            var service = new ServiceA();
            
            // Act
            var result = injector.TryRegister(typeof(IServiceA), service);
            
            // Assert
            Assert.That(result, Is.True);
            Assert.That(injector.IsRegistered<IServiceA>(), Is.True);
            Assert.That(injector.Resolve<IServiceA>(), Is.EqualTo(service));
        }
        
        [Test]
        public void TryRegister_WithNullInstance_ShouldThrowException()
        {
            // Arrange
            var injector = new SInjector();
            
            // Act
            void Act() => injector.TryRegister(typeof(IServiceA), null);
            
            // Assert
            Assert.That(Act, Throws.ArgumentNullException);
            Assert.That(injector.IsRegistered<IServiceA>(), Is.False);
        }
        
        [Test]
        public void TryRegister_WithNullType_ShouldThrowException()
        {
            // Arrange
            var injector = new SInjector();
            var service = new ServiceA();
            
            // Act
            void Act() => injector.TryRegister(null, service);
            
            // Assert
            Assert.That(Act, Throws.ArgumentNullException);
            Assert.That(injector.IsRegistered<ServiceA>(), Is.False);
        }
        
        [Test]
        public void TryRegister_WithAlreadyRegistered_ShouldRegisterSecondInstance()
        {
            // Arrange
            var injector = new SInjector();
            var service = new ServiceA();
            var service2 = new ServiceA();
            injector.Register(typeof(IServiceA), service);
            
            // Act
            var result = injector.TryRegister(typeof(IServiceA), service2);
            
            // Assert
            Assert.That(result, Is.True);
            Assert.That(injector.IsRegistered<IServiceA>(), Is.True);
            Assert.That(injector.Resolve<IServiceA>(), Is.EqualTo(service));
            Assert.That(injector.RegisteredInstances, Does.Contain(service));
            Assert.That(injector.RegisteredInstances, Does.Contain(service2));
        }
        
        [Test]
        public void TryRegister_WithDifferentType_ShouldThrowException()
        {
            // Arrange
            var injector = new SInjector();
            var service = new ServiceA();
            
            // Act
            void Act() => injector.TryRegister(typeof(IServiceB), service);
            
            // Assert
            Assert.That(Act, Throws.ArgumentException);
            Assert.That(injector.IsRegistered<IServiceA>(), Is.False);
            Assert.That(injector.IsRegistered<IServiceB>(), Is.False);
            Assert.That(injector.IsRegistered<ServiceA>(), Is.False);
        }
        
        [Test]
        public void TryRegister_WithAlreadyRegisteredAndThenDestroyedInstance_ShouldRegisterAndReturnTrue()
        {
            // Arrange
            var injector = new SInjector();
            var service = ScriptableObject.CreateInstance<ScriptableServiceA>();
            var service2 = ScriptableObject.CreateInstance<ScriptableServiceA>();
            injector.Register(typeof(IServiceA), service);
            
            Object.DestroyImmediate(service);

            // Act
            var result = injector.TryRegister(typeof(IServiceA), service2);
            
            // Assert
            Assert.That(result, Is.True);
            Assert.That(injector.IsRegistered<IServiceA>(), Is.True);
            Assert.That(injector.Resolve<IServiceA>(), Is.EqualTo(service2));
        }
        
        [Test]
        public void TryRegister_WithDestroyedInstance_ShouldNotRegisterAndReturnFalse()
        {
            // Arrange
            var injector = new SInjector();
            var service = ScriptableObject.CreateInstance<ScriptableServiceA>();
            Object.DestroyImmediate(service);
            
            // Act
            var result = injector.TryRegister(typeof(IServiceA), service);
            
            // Assert
            Assert.That(result, Is.False);
            Assert.That(injector.IsRegistered<IServiceA>(), Is.False);
            Assert.That(injector.RegisteredInstances, Does.Not.Contains(service));
        }
        
        #endregion

        #region Unregister<T>

        [Test]
        public void GenericUnregister_ShouldUnregister()
        {
            // Arrange
            var injector = new SInjector();
            var service = new ServiceA();
            injector.Register<IServiceA>(service);
            
            // Act
            injector.Unregister<IServiceA>(service);
            
            // Assert
            Assert.That(injector.IsRegistered<IServiceA>(), Is.False);
        }
        
        [Test]
        public void GenericUnregister_WithNullInstance_ShouldThrowException()
        {
            // Arrange
            var injector = new SInjector();
            
            // Act
            void Act() => injector.Unregister<IServiceA>(null);
            
            // Assert
            Assert.That(Act, Throws.ArgumentNullException);
            Assert.That(injector.IsRegistered<IServiceA>(), Is.False);
        }
        
        [Test]
        public void GenericUnregister_WithNotRegisteredInstance_ShouldNotThrowException()
        {
            // Arrange
            var injector = new SInjector();
            var service = new ServiceA();
            
            // Act
            void Act() => injector.Unregister<IServiceA>(service);
            
            // Assert
            Assert.That(Act, Throws.Nothing);
            Assert.That(injector.IsRegistered<IServiceA>(), Is.False);
        }

        [Test]
        public void GenericUnregister_WithDifferentInstance_ShouldNotUnregister()
        {
            // Arrange
            var injector = new SInjector();
            var service = new ServiceA();
            var service2 = new ServiceA();
            injector.Register<IServiceA>(service);
            
            // Act
            injector.Unregister<IServiceA>(service2);
            
            // Assert
            Assert.That(injector.IsRegistered<IServiceA>(), Is.True);
            Assert.That(injector.Resolve<IServiceA>(), Is.EqualTo(service));
        }

        #endregion
        
        #region Unregister
        
        [Test]
        public void Unregister_ShouldUnregister()
        {
            // Arrange
            var injector = new SInjector();
            var service = new ServiceA();
            injector.Register(typeof(IServiceA), service);
            
            // Act
            injector.Unregister(typeof(IServiceA), service);
            
            // Assert
            Assert.That(injector.IsRegistered<IServiceA>(), Is.False);
        }
        
        [Test]
        public void Unregister_WithNullInstance_ShouldThrowException()
        {
            // Arrange
            var injector = new SInjector();
            
            // Act
            void Act() => injector.Unregister(typeof(IServiceA), null);
            
            // Assert
            Assert.That(Act, Throws.ArgumentNullException);
            Assert.That(injector.IsRegistered<IServiceA>(), Is.False);
        }
        
        [Test]
        public void Unregister_WithNotRegisteredInstance_ShouldNotThrowException()
        {
            // Arrange
            var injector = new SInjector();
            var service = new ServiceA();
            
            // Act
            void Act() => injector.Unregister(typeof(IServiceA), service);
            
            // Assert
            Assert.That(Act, Throws.Nothing);
            Assert.That(injector.IsRegistered<IServiceA>(), Is.False);
        }
        
        [Test]
        public void Unregister_WithNullType_ShouldThrowException()
        {
            // Arrange
            var injector = new SInjector();
            var service = new ServiceA();
            
            // Act
            void Act() => injector.Unregister(null, service);
            
            // Assert
            Assert.That(Act, Throws.ArgumentNullException);
            Assert.That(injector.IsRegistered<ServiceA>(), Is.False);
        }
        
        [Test]
        public void Unregister_WithDifferentInstance_ShouldNotUnregister()
        {
            // Arrange
            var injector = new SInjector();
            var service = new ServiceA();
            var service2 = new ServiceA();
            injector.Register(typeof(IServiceA), service);
            
            // Act
            injector.Unregister(typeof(IServiceA), service2);
            
            // Assert
            Assert.That(injector.IsRegistered<IServiceA>(), Is.True);
            Assert.That(injector.Resolve<IServiceA>(), Is.EqualTo(service));
        }
        
        #endregion

        #region IsRegistered<T>

        [Test]
        public void GenericIsRegistered_WithRegisteredInstance_ShouldReturnTrue()
        {
            // Arrange
            var injector = new SInjector();
            var service = new ServiceA();
            injector.Register<IServiceA>(service);
            
            // Act
            var result = injector.IsRegistered<IServiceA>();
            
            // Assert
            Assert.That(result, Is.True);
        }
        
        [Test]
        public void GenericIsRegistered_WithNotRegisteredInstance_ShouldReturnFalse()
        {
            // Arrange
            var injector = new SInjector();
            
            // Act
            var result = injector.IsRegistered<IServiceA>();
            
            // Assert
            Assert.That(result, Is.False);
        }
        
        [Test]
        public void GenericIsRegistered_WithDestroyedInstance_ShouldReturnFalse()
        {
            // Arrange
            var injector = new SInjector();
            var service = ScriptableObject.CreateInstance<ScriptableServiceA>();
            injector.Register<IServiceA>(service);
            Object.DestroyImmediate(service);
            
            // Act
            var result = injector.IsRegistered<IServiceA>();
            
            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region IsRegistered

        [Test]
        public void IsRegistered_WithRegisteredInstance_ShouldReturnTrue()
        {
            // Arrange
            var injector = new SInjector();
            var service = new ServiceA();
            injector.Register(typeof(IServiceA), service);
            
            // Act
            var result = injector.IsRegistered(typeof(IServiceA));
            
            // Assert
            Assert.That(result, Is.True);
        }
        
        [Test]
        public void IsRegistered_WithNotRegisteredInstance_ShouldReturnFalse()
        {
            // Arrange
            var injector = new SInjector();
            
            // Act
            var result = injector.IsRegistered(typeof(IServiceA));
            
            // Assert
            Assert.That(result, Is.False);
        }
        
        [Test]
        public void IsRegistered_WithDestroyedInstance_ShouldReturnFalse()
        {
            // Arrange
            var injector = new SInjector();
            var service = ScriptableObject.CreateInstance<ScriptableServiceA>();
            injector.Register(typeof(IServiceA), service);
            Object.DestroyImmediate(service);
            
            // Act
            var result = injector.IsRegistered(typeof(IServiceA));
            
            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region Resolve<T>

        [Test]
        public void GenericResolve_WithRegisteredInstance_ShouldReturnInstance()
        {
            // Arrange
            var injector = new SInjector();
            var service = new ServiceA();
            injector.Register<IServiceA>(service);
            
            // Act
            var result = injector.Resolve<IServiceA>();
            
            // Assert
            Assert.That(result, Is.EqualTo(service));
        }
        
        [Test]
        public void GenericResolve_WithNotRegisteredInstance_ShouldReturnNull()
        {
            // Arrange
            LogAssert.ignoreFailingMessages = true;
            var injector = new SInjector();
            
            // Act
            var result = injector.Resolve<IServiceA>();
            
            // Assert
            Assert.That(result, Is.Null);
        }
        
        [Test]
        public void GenericResolve_WithDestroyedInstance_ShouldReturnNull()
        {
            // Arrange
            LogAssert.ignoreFailingMessages = true;
            var injector = new SInjector();
            var service = ScriptableObject.CreateInstance<ScriptableServiceA>();
            injector.Register<IServiceA>(service);
            Object.DestroyImmediate(service);
            
            // Act
            var result = injector.Resolve<IServiceA>();
            
            // Assert
            Assert.That(result, Is.Null);
        }
        
        #endregion

        #region Resolve

        [Test]
        public void Resolve_WithRegisteredInstance_ShouldReturnInstance()
        {
            // Arrange
            var injector = new SInjector();
            var service = new ServiceA();
            injector.Register(typeof(IServiceA), service);
            
            // Act
            var result = injector.Resolve(typeof(IServiceA));
            
            // Assert
            Assert.That(result, Is.EqualTo(service));
        }
        
        [Test]
        public void Resolve_WithNotRegisteredInstance_ShouldReturnNull()
        {
            // Arrange
            LogAssert.ignoreFailingMessages = true;
            var injector = new SInjector();
            
            // Act
            var result = injector.Resolve(typeof(IServiceA));
            
            // Assert
            Assert.That(result, Is.Null);
        }
        
        [Test]
        public void Resolve_WithDestroyedInstance_ShouldReturnNull()
        {
            // Arrange
            LogAssert.ignoreFailingMessages = true;
            var injector = new SInjector();
            var service = ScriptableObject.CreateInstance<ScriptableServiceA>();
            injector.Register(typeof(IServiceA), service);
            Object.DestroyImmediate(service);
            
            // Act
            var result = injector.Resolve(typeof(IServiceA));
            
            // Assert
            Assert.That(result, Is.Null);
        }
        
        [Test]
        public void Resolve_WithNullType_ShouldThrowException()
        {
            // Arrange
            var injector = new SInjector();
            
            // Act
            void Act() => injector.Resolve(null);
            
            // Assert
            Assert.That(Act, Throws.ArgumentNullException);
        }

        #endregion

        #region TryResolve<T>

        [Test]
        public void GenericTryResolve_WithRegisteredInstance_ShouldReturnTrueAndInstance()
        {
            // Arrange
            var injector = new SInjector();
            var service = new ServiceA();
            injector.Register<IServiceA>(service);
            
            // Act
            var result = injector.TryResolve<IServiceA>(out var instance);
            
            // Assert
            Assert.That(result, Is.True);
            Assert.That(instance, Is.EqualTo(service));
        }
        
        [Test]
        public void GenericTryResolve_WithNotRegisteredInstance_ShouldReturnFalseAndNull()
        {
            // Arrange
            var injector = new SInjector();
            
            // Act
            var result = injector.TryResolve<IServiceA>(out var instance);
            
            // Assert
            Assert.That(result, Is.False);
            Assert.That(instance, Is.Null);
        }
        
        [Test]
        public void GenericTryResolve_WithDestroyedInstance_ShouldReturnFalseAndNull()
        {
            // Arrange
            var injector = new SInjector();
            var service = ScriptableObject.CreateInstance<ScriptableServiceA>();
            injector.Register<IServiceA>(service);
            Object.DestroyImmediate(service);
            
            // Act
            var result = injector.TryResolve<IServiceA>(out var instance);
            
            // Assert
            Assert.That(result, Is.False);
            Assert.That(instance, Is.Null);
        }

        #endregion
        
        #region TryResolve
        
        [Test]
        public void TryResolve_WithRegisteredInstance_ShouldReturnTrueAndInstance()
        {
            // Arrange
            var injector = new SInjector();
            var service = new ServiceA();
            injector.Register(typeof(IServiceA), service);
            
            // Act
            var result = injector.TryResolve(typeof(IServiceA), out var instance);
            
            // Assert
            Assert.That(result, Is.True);
            Assert.That(instance, Is.EqualTo(service));
        }
        
        [Test]
        public void TryResolve_WithNotRegisteredInstance_ShouldReturnFalseAndNull()
        {
            // Arrange
            var injector = new SInjector();
            
            // Act
            var result = injector.TryResolve(typeof(IServiceA), out var instance);
            
            // Assert
            Assert.That(result, Is.False);
            Assert.That(instance, Is.Null);
        }
        
        [Test]
        public void TryResolve_WithDestroyedInstance_ShouldReturnFalseAndNull()
        {
            // Arrange
            var injector = new SInjector();
            var service = ScriptableObject.CreateInstance<ScriptableServiceA>();
            injector.Register(typeof(IServiceA), service);
            Object.DestroyImmediate(service);
            
            // Act
            var result = injector.TryResolve(typeof(IServiceA), out var instance);
            
            // Assert
            Assert.That(result, Is.False);
            Assert.That(instance, Is.Null);
        }
        
        [Test]
        public void TryResolve_WithNullType_ShouldThrowException()
        {
            // Arrange
            var injector = new SInjector();
            
            // Act
            void Act() => injector.TryResolve(null, out _);
            
            // Assert
            Assert.That(Act, Throws.ArgumentNullException);
        }

        [Test]
        public void TryResolve_ShouldResolveArray()
        {
            // Arrange
            var injector = new SInjector();
            var serviceA = new ServiceA();
            var serviceB = new ServiceB();
            
            injector.Register(typeof(IServiceA), serviceA);
            injector.Register(typeof(IServiceB), serviceB);
            
            // Act
            var result = injector.TryResolve(typeof(IServiceA[]), out var array);
            
            // Assert
            Assert.That(result, Is.True);
            Assert.That(array, Is.TypeOf<IServiceA[]>());
            Assert.That(array, Has.Length.EqualTo(1));
            Assert.That(array, Contains.Item(serviceA));
        }
        
        [Test]
        public void TryResolve_ShouldResolveList()
        {
            // Arrange
            var injector = new SInjector();
            var serviceA = new ServiceA();
            var serviceB = new ServiceB();
            
            injector.Register(typeof(IServiceA), serviceA);
            injector.Register(typeof(IServiceB), serviceB);
            
            // Act
            var result = injector.TryResolve(typeof(List<IServiceA>), out var list);
            
            // Assert
            Assert.That(result, Is.True);
            Assert.That(list, Is.TypeOf<List<IServiceA>>());
            Assert.That(list, Has.Count.EqualTo(1));
            Assert.That(list, Contains.Item(serviceA));
        }
        
        [Test]
        public void TryResolve_ShouldResolveEnumerable()
        {
            // Arrange
            var injector = new SInjector();
            var serviceA = new ServiceA();
            var serviceB = new ServiceB();
            
            injector.Register(typeof(IServiceA), serviceA);
            injector.Register(typeof(IServiceB), serviceB);
            
            // Act
            var result = injector.TryResolve(typeof(IEnumerable<IServiceA>), out var enumerable);
            
            // Assert
            Assert.That(result, Is.True);
            Assert.That(enumerable, Is.AssignableTo<IEnumerable<IServiceA>>());
            Assert.That(enumerable, Contains.Item(serviceA));
        }
        
        
        #endregion

        #region ResolveAll

        [Test]
        public void ResolveAll_ShouldResolveAllInstances()
        {
            // Arrange
            var injector = new SInjector();
            var service1 = new ServiceA();
            var service2 = new ServiceA();
            injector.Register<IServiceA>(service1);
            injector.Register<IServiceA>(service2);
            
            var result = new HashSet<object>();
            
            // Act
            injector.ResolveAll(typeof(IServiceA), result);
            
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.Empty);
            Assert.That(result, Contains.Item(service1));
            Assert.That(result, Contains.Item(service2));
            Assert.That(result, Has.Count.EqualTo(2));
        }


        #endregion
        
        #region Inject

        [Test]
        public void Inject_WithPrivateField_ShouldInject()
        {
            // Arrange
            var injector = new SInjector();
            var injectable = new PrivateFieldInjectable();
            var service = new ServiceA();
            injector.Register<IServiceA>(service);
            
            // Act
            injector.Inject(injectable);
            
            // Assert
            Assert.That(injectable.Service, Is.EqualTo(service));
        }
        
        [Test]
        public void Inject_WithPublicField_ShouldInject()
        {
            // Arrange
            var injector = new SInjector();
            var injectable = new PublicFieldInjectable();
            var service = new ServiceA();
            injector.Register<IServiceA>(service);
            
            // Act
            injector.Inject(injectable);
            
            // Assert
            Assert.That(injectable.Service, Is.EqualTo(service));
        }
        
        [Test]
        public void Inject_WithMultipleFields_ShouldInject()
        {
            // Arrange
            var injector = new SInjector();
            var injectable = new MultipleFieldsInjectable();
            var serviceA = new ServiceA();
            var serviceB = new ServiceB();
            injector.Register<IServiceA>(serviceA);
            injector.Register<IServiceB>(serviceB);
            
            // Act
            injector.Inject(injectable);
            
            // Assert
            Assert.That(injectable.ServiceA, Is.EqualTo(serviceA));
            Assert.That(injectable.ServiceB, Is.EqualTo(serviceB));
        }
        
        [Test]
        public void Inject_WithNotRegisteredInstance_ShouldNotThrowException()
        {
            // Arrange
            LogAssert.ignoreFailingMessages = true;
            var injector = new SInjector();
            var injectable = new PrivateFieldInjectable();
            
            // Act
            void Act() => injector.Inject(injectable);
            
            // Assert
            Assert.That(Act, Throws.Nothing);
            Assert.That(injectable.Service, Is.Null);
        }
        
        [Test]
        public void Inject_WithNoInjectableFields_ShouldNotThrowException()
        {
            // Arrange
            var injector = new SInjector();
            var injectable = new NoInjectableFieldsInjectable();
            var service = new ServiceA();
            injector.Register<IServiceA>(service);
            
            // Act
            void Act() => injector.Inject(injectable);
            
            // Assert
            Assert.That(Act, Throws.Nothing);
        }
        
        [Test]
        public void Inject_WithInjectorField_ShouldInject()
        {
            // Arrange
            var injector = new SInjector();
            var injectable = new InjectorFieldInjectable();
            
            // Act
            injector.Inject(injectable);
            
            // Assert
            Assert.That(injectable.Injector, Is.EqualTo(injector));
        }

        #endregion

        #region get_RegisteredInstances

        [Test]
        public void get_RegisteredInstances_ShouldReturnNotNull()
        {
            // Arrange
            var injector = new SInjector();
            
            // Act
            var result = injector.RegisteredInstances;
            
            // Assert
            Assert.That(result, Is.Not.Null);
        }
        
        [Test]
        public void get_RegisteredInstances_WithNoRegisteredInstance_ShouldReturnInjector()
        {
            // Arrange
            var injector = new SInjector();
            
            // Act
            var result = injector.RegisteredInstances.ToArray();
            
            // Assert
            Assert.That(result, Has.Length.EqualTo(1));
            Assert.That(result, Has.Member(injector));
        }
        
        [Test]
        public void get_RegisteredInstances_WithRegisteredInstance_ShouldReturnNotNull()
        {
            // Arrange
            var injector = new SInjector();
            var service = new ServiceA();
            injector.Register<IServiceA>(service);
            
            // Act
            var result = injector.RegisteredInstances;
            
            // Assert
            Assert.That(result, Is.Not.Null);
        }
        
        [Test]
        public void get_RegisteredInstances_WithRegisteredInstance_ShouldReturnInstance()
        {
            // Arrange
            var injector = new SInjector();
            var service = new ServiceA();
            injector.Register<IServiceA>(service);
            
            // Act
            var result = injector.RegisteredInstances.ToArray();
            
            // Assert
            Assert.That(result, Has.Length.EqualTo(2));
            Assert.That(result, Has.Member(service));
        }
        
        [Test]
        public void get_RegisteredInstances_WithRegisteredInstances_ShouldReturnInstances()
        {
            // Arrange
            var injector = new SInjector();
            var serviceA = new ServiceA();
            var serviceB = new ServiceB();
            injector.Register<IServiceA>(serviceA);
            injector.Register<IServiceB>(serviceB);
            
            // Act
            var result = injector.RegisteredInstances.ToArray();
            
            // Assert
            Assert.That(result, Has.Length.EqualTo(3));
            Assert.That(result, Has.Member(serviceA));
            Assert.That(result, Has.Member(serviceB));
        }

        #endregion

        #region PerformSelfInjection

        [Test]
        public void PerformSelfInjection_WithSelfDependency_ShouldInject()
        {
            // Arrange
            var injector = new SInjector();
            var service = new ServiceAWithDependencyToA();
            injector.Register<IServiceA>(service);
            
            // Act
            injector.PerformSelfInjection();
            
            // Assert
            Assert.That(service.ServiceA, Is.EqualTo(service));
        }
        
        [Test]
        public void PerformSelfInjection_WithCircularDependency_ShouldInject()
        {
            // Arrange
            var injector = new SInjector();
            var serviceA = new ServiceAWithDependencyToB();
            var serviceB = new ServiceBWithDependencyToA();
            injector.Register<IServiceA>(serviceA);
            injector.Register<IServiceB>(serviceB);
            
            // Act
            injector.PerformSelfInjection();
            
            // Assert
            Assert.That(serviceA.ServiceB, Is.EqualTo(serviceB));
            Assert.That(serviceB.ServiceA, Is.EqualTo(serviceA));
        }
        
        [Test]
        public void PerformSelfInjection_WithSelfDependencyAndNotRegisteredInstance_ShouldNotThrowException()
        {
            // Arrange
            LogAssert.ignoreFailingMessages = true;
            var injector = new SInjector();
            var service = new ServiceAWithDependencyToB();
            
            // Act
            void Act() => injector.PerformSelfInjection();
            
            // Assert
            Assert.That(Act, Throws.Nothing);
            Assert.That(service.ServiceB, Is.Null);
        }
        
        [Test]
        public void PerformSelfInjection_WithOneWayDependency_ShouldInject()
        {
            // Arrange
            var injector = new SInjector();
            var serviceA = new ServiceAWithDependencyToB();
            var serviceB = new ServiceB();
            injector.Register<IServiceA>(serviceA);
            injector.Register<IServiceB>(serviceB);
            
            // Act
            injector.PerformSelfInjection();
            
            // Assert
            Assert.That(serviceA.ServiceB, Is.EqualTo(serviceB));
        }

        #endregion
    }
}
