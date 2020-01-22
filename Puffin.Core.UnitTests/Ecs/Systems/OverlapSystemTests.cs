using System;
using NUnit.Framework;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Systems;

namespace Puffin.Core.UnitTests.Ecs.Systems
{
    [TestFixture]
    public class OverlapSystemTests
    {
        [Test]
        public void OnUpdateTriggersStartedOverlapping()
        {
            var e1Called = false;
            var e2Called = false;

            // Arrange
            var e1 = new Entity().Move(16, 16).Overlap(32, 32, 0, 0, (o) => e1Called = true, null);
            var e2 = new Entity().Overlap(40, 40, 0, 0, (o) => e2Called = true, null);

            var system = new OverlapSystem();
            system.OnAddEntity(e1);
            system.OnAddEntity(e2);

            // Act
            system.OnUpdate(TimeSpan.Zero);

            // Assert
            Assert.That(e1Called, Is.True);
            Assert.That(e2Called, Is.True);
        }

        [Test]
        public void OnUpdateTriggersStoppedOverlapping()
        {
            var e1Called = false;
            var e2Called = false;

            // Arrange
            var e1 = new Entity().Move(16, 16).Overlap(32, 32, 0, 0, null, (o) => e1Called = true);
            var e2 = new Entity().Overlap(40, 40, 0, 0, null, (o) => e2Called = true);

            var system = new OverlapSystem();
            system.OnAddEntity(e1);
            system.OnAddEntity(e2);
            
            // Trigger overlap
            system.OnUpdate(TimeSpan.Zero);

            e1.Move(-100, -100);

            // Act
            system.OnUpdate(TimeSpan.Zero);

            // Assert
            Assert.That(e1Called, Is.True);
            Assert.That(e2Called, Is.True);
        }
    }
}