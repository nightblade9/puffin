using System;
using Moq;
using NUnit.Framework;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Core.Ecs.Systems;
using Puffin.Core.IO;

namespace Puffin.Core.UnitTests.Ecs
{
    [TestFixture]
    public class FourWayMovementComponentTests
    {
        
        [Test]
        public void OnUpdateSetsVelocityProportionalToVelocityAndTimeElapsed()
        {
            const int speed = 100;
            var elapsed = TimeSpan.FromSeconds(2);

            // Depends on default mapping for PuffinGame.
            // Arrange
            var provider = new Mock<IKeyboardProvider>();
            provider.Setup(p => p.IsActionDown(PuffinAction.Down)).Returns(true);
            provider.Setup(p => p.IsActionDown(PuffinAction.Left)).Returns(true);
            new Scene().Initialize(new ISystem[] { new Mock<DrawingSystem>().Object }, null, provider.Object);

            // Depends on default action bindings
            var e = new Entity();
            var component = new FourWayMovementComponent(e, speed);

            // Act
            component.OnUpdate();
            
            // Assert
            Assert.That(e.VelocityX, Is.EqualTo(-speed));
            Assert.That(e.VelocityY, Is.EqualTo(speed));
        }
    }
}