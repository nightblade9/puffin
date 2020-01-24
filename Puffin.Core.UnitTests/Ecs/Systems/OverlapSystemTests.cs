using System;
using NUnit.Framework;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
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

        [Test]
        public void StartedOverlappingFiresEventOnFirstCall()
        {
            // Arrange
            var onStartCalled = false;
            var overlap = new OverlapComponent(new Entity(), 32, 32, 0, 0, (o) => onStartCalled = true, null);

            var target = new Entity().Overlap(32, 32);

            // Act
            OverlapSystem.StartedOverlapping(overlap, target);

            // Assert
            Assert.That(onStartCalled, Is.True);
        }

        [Test]
        public void StoppedOverlappingFiresEventOnFirstCall()
        {
            var onStopCalled = false;

            // Arrange
            var overlap = new OverlapComponent(new Entity(), 32, 32, 0, 0, null, (o) => onStopCalled = true);

            var e = new Entity();
            var target = new OverlapComponent(e, 32, 32);

            // Act
            OverlapSystem.StartedOverlapping(overlap, e);
            OverlapSystem.StoppedOverlapping(overlap, e);

            // Assert
            Assert.That(onStopCalled, Is.True);
        }

        [Test]
        public void StartStopStartOverlapAndItFiresThreeEvents()
        {
            var isOverlapping = false;

            // Arrange
            var overlap = new OverlapComponent(new Entity(), 32, 32, 0, 0, (o) => isOverlapping = true, (o) => isOverlapping = false);

            var e = new Entity();
            var target = new OverlapComponent(e, 32, 32);
            OverlapSystem.StartedOverlapping(overlap, e);

            // Act
            OverlapSystem.StoppedOverlapping(overlap, e);
            Assert.That(isOverlapping, Is.False);

            OverlapSystem.StartedOverlapping(overlap, e);

            // Assert
            Assert.That(isOverlapping, Is.True);
        }

        [Test]
        public void StartOverlappingDoesntTriggerCallbackIfCalledMultipleTimes()
        {
            var timesCalled = 0;

            // Arrange
            var overlap = new OverlapComponent(new Entity(), 32, 32, 0, 0, (o) => timesCalled++, null);

            var e = new Entity();
            var target = new OverlapComponent(e, 32, 32);

            // Act
            OverlapSystem.StartedOverlapping(overlap, e);
            OverlapSystem.StartedOverlapping(overlap, e);
            OverlapSystem.StartedOverlapping(overlap, e);
            OverlapSystem.StartedOverlapping(overlap, e);

            // Assert
            Assert.That(timesCalled, Is.EqualTo(1));
        }

        [Test]
        public void StoppedOverlappingDoesntTriggerCallbackIfStartOverlappingWasntCalled()
        {
            var onStopCalled = false;

            // Arrange
            var overlap = new OverlapComponent(new Entity(), 32, 32, 0, 0, null, (o) => onStopCalled = true);

            var e = new Entity();
            var target = new OverlapComponent(e, 32, 32);

            // Act
            OverlapSystem.StoppedOverlapping(overlap, e);

            // Assert
            Assert.That(onStopCalled, Is.False);
        }

        [Test]
        public void StoppedOverlappingDoesntTriggerCallbackIfCalledMultipleTimes()
        {
            var timesCalled = 0;

            // Arrange
            var overlap = new OverlapComponent(new Entity(), 32, 32, 0, 0, (o) => timesCalled++, null);

            var e = new Entity();
            var target = new OverlapComponent(e, 32, 32);
            OverlapSystem.StartedOverlapping(overlap, e);

            // Act
            OverlapSystem.StoppedOverlapping(overlap, e);
            OverlapSystem.StoppedOverlapping(overlap, e);
            OverlapSystem.StoppedOverlapping(overlap, e);
            OverlapSystem.StoppedOverlapping(overlap, e);

            // Assert
            Assert.That(timesCalled, Is.EqualTo(1));
        }
    }
}