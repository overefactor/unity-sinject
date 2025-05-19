using NUnit.Framework;
using Sapo.DI.Runtime.Interfaces;
using Sapo.DI.Samples.Testing.Runtime;
using Sapo.DI.Tests.Runtime.Helpers;

namespace Sapo.DI.Samples.Testing.Tests.Runtime
{
    [TestFixture]
    public class PlayerTests
    {
        private class TestHealth : IHealth
        {
            public int Value { get; set; }
        } 
        
        private STestGameObjectHelper _gHelper;

        [SetUp]
        public void SetUp()
        {
            _gHelper = new STestGameObjectHelper();
        }
        
        [TearDown]
        public void TearDown()
        {
            _gHelper.Dispose();
        }
        
        [Test]
        public void TakeDamage_With10Damage_ShouldReduceHealthBy10()
        {
            // Arrange
            var playerG = _gHelper.CreateG<Player>();
            var player = playerG.GetComponent<Player>();
            var injector = new STestGameObjectInjector(playerG);
            
            var health = new TestHealth { Value = 100 };
            injector.Register<IHealth>(health);
            injector.Activate();
            
            // Act
            player.TakeDamage(10);
            
            // Assert
            Assert.That(health.Value, Is.EqualTo(90));
        }
    }
}