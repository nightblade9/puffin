using System;
using Moq;
using NUnit.Framework;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Systems;
using Puffin.Core.IO;

namespace Puffin.Core.UnitTests.Ecs.Systems
{
    [TestFixture]
    public class MouseOverlapSystemTests
    {
        [Test]
        public void OnUpdateFiresMouseEnterCallback()
        {
            // Arrange
            var called = false;
            var provider = new Mock<IMouseProvider>();
            provider.Setup(p => p.MouseCoordinates).Returns(new System.Tuple<int, int>(30, 20));
            var system = new MouseOverlapSystem(provider.Object);
            
            var e = new Entity().Move(25, 15).Overlap(10, 10, 0, 0, () => called = true);
            system.OnAddEntity(e);

            // Act
            system.OnUpdate(TimeSpan.FromSeconds(1));

            // Assert
            Assert.That(called, Is.True);
        }

        [Test]
        public void RemoveRemovesEntity()
        {
            // Doesn't fire event

            // Arrange
            var called = false;
            var provider = new Mock<IMouseProvider>();
            provider.Setup(p => p.MouseCoordinates).Returns(new System.Tuple<int, int>(30, 20));
            var system = new MouseOverlapSystem(provider.Object);
            
            var e = new Entity().Move(25, 15).Overlap(10, 10, 0, 0, () => called = true);
            system.OnAddEntity(e);

            // Act
            system.OnRemoveEntity(e);
            system.OnUpdate(TimeSpan.FromSeconds(1));

            // Assert
            Assert.That(called, Is.False);
        }

        [Test]
        public void OnUpdateFiresMouseExitCallback()
        {
            // Arrange
            var called = false;
            var provider = new Mock<IMouseProvider>();
            provider.Setup(p => p.MouseCoordinates).Returns(new System.Tuple<int, int>(30, 20));
            var system = new MouseOverlapSystem(provider.Object);
            
            var e = new Entity().Move(25, 15).Overlap(10, 10, 0, 0, null, () => called = true);
            system.OnAddEntity(e);
            system.OnUpdate(TimeSpan.FromSeconds(1)); // add to "mouse overlapping"

            provider.Setup(p => p.MouseCoordinates).Returns(new Tuple<int, int>(0, 0));

            // Act
            system.OnUpdate(TimeSpan.FromSeconds(1));

            // Assert
            Assert.That(called, Is.True);
        }
    }
}