using System;
using Moq;
using NUnit.Framework;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Core.IO;

namespace Puffin.Core.UnitTests.Ecs
{
    [TestFixture]
    public class FourWayMovementComponentTests
    {
        [TearDown]
        public void ResetDependencyInjectionBindings()
        {
            DependencyInjection.Reset();
        }

        [Test]
        public void OnUpdateSetsParentIntentProportionalToVelocityAndTimeElapsed()
        {
            const int speed = 100;
            var elapsed = TimeSpan.FromSeconds(2);

            // Depends on default mapping for PuffinGame.
            // Arrange
            var provider = new Mock<IKeyboardProvider>();
            DependencyInjection.Kernel.Bind<IKeyboardProvider>().ToConstant(provider.Object);
            provider.Setup(p => p.IsActionDown(PuffinAction.Down)).Returns(true);
            provider.Setup(p => p.IsActionDown(PuffinAction.Left)).Returns(true);

            // Depends on default action bindings
            var e = new Entity();
            var component = new FourWayMovementComponent(e, speed);

            // Act
            component.OnUpdate(elapsed);
            
            // Assert
            Assert.That(e.X, Is.EqualTo(-speed * elapsed.TotalSeconds));
            Assert.That(e.Y, Is.EqualTo(speed * elapsed.TotalSeconds));
        }
    }
}