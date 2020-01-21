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
            var e1 = new Entity().Move(16, 16);
            e1.Set(new OverlapComponent(e1, 32, 32, 0, 0, (o) => e1Called = true, null));

            var e2 = new Entity();
            e2.Set(new OverlapComponent(e2, 40, 40, 0, 0, (o) => e2Called = true, null));

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

        }
    }
}