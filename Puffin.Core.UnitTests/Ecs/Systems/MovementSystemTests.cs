using System;
using Moq;
using NUnit.Framework;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Systems;
using Puffin.Core.IO;

namespace Puffin.Core.UnitTests
{
    [TestFixture]
    public class MovementSystemTests
    {
        [TearDown]
        public void ResetDependencyInjectionBindings()
        {
            DependencyInjection.Reset();
        }
        
        
        [Test]
        public void OnUpdateCallsOnUpdateOnFourWayComponents()
        {
            // Arrange
            const int speed = 100;
            var elapsed = TimeSpan.FromSeconds(2);

            // Depends on default mapping for PuffinGame.
            // Arrange
            var provider = new Mock<IKeyboardProvider>();
            DependencyInjection.Kernel.Bind<IKeyboardProvider>().ToConstant(provider.Object);
            provider.Setup(p => p.IsActionDown(PuffinAction.Up)).Returns(true);
            provider.Setup(p => p.IsActionDown(PuffinAction.Right)).Returns(true);

            var e1 = new Entity().FourWayMovement(100);
            var e2 = new Entity().Sprite("flower.png");

            var system = new MovementSystem();
            system.OnAddEntity(e1);
            system.OnAddEntity(e2);

            // Act
            system.OnUpdate(TimeSpan.FromSeconds(1));

            // Assert
            // e1 has a movement component so it moved
            Assert.That(e1.X, Is.EqualTo(speed));
            Assert.That(e1.Y, Is.EqualTo(-speed));
            
            // e2 doesn't have a movement component so it's at the start position
            Assert.That(e2.X, Is.EqualTo(0));
            Assert.That(e2.Y, Is.EqualTo(0));

        }

        [Test]
        public void OnRemoveRemovesEntity()
        {
            var provider = new Mock<IKeyboardProvider>();
            DependencyInjection.Kernel.Bind<IKeyboardProvider>().ToConstant(provider.Object);
            provider.Setup(p => p.IsActionDown(PuffinAction.Up)).Returns(true);
            provider.Setup(p => p.IsActionDown(PuffinAction.Right)).Returns(true);

            var e = new Entity().FourWayMovement(100);
            var system = new MovementSystem();
            system.OnAddEntity(e);
            system.OnUpdate(TimeSpan.FromSeconds(1));
            var expectedPosition = new Tuple<float, float>(e.X, e.Y);

            // Act
            system.OnRemoveEntity(e);
            system.OnUpdate(TimeSpan.FromSeconds(10));

            // Assert: removed entities don't move
            Assert.That(e.X, Is.EqualTo(expectedPosition.Item1));
            Assert.That(e.Y, Is.EqualTo(expectedPosition.Item2));
        }
    }
}