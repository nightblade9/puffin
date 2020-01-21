using NUnit.Framework;
using Puffin.Core.Ecs;

namespace Puffin.Core.UnitTests.Ecs.Components
{
    [TestFixture]
    public class OverlapComponentTests
    {
        [Test]
        public void StartedOverlappingFiresEventOnFirstCall()
        {
            var onStartCalled = false;

            // Arrange
            var overlap = new OverlapComponent(new Entity(), 32, 32, 0, 0, (o) => onStartCalled = true, null);

            var e = new Entity();
            var target = new OverlapComponent(e, 32, 32);

            // Act
            overlap.StartedOverlapping(e);

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
            overlap.StartedOverlapping(e);
            overlap.StoppedOverlapping(e);

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
            overlap.StartedOverlapping(e);

            // Act
            overlap.StoppedOverlapping(e);
            Assert.That(isOverlapping, Is.False);

            overlap.StartedOverlapping(e);

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
            overlap.StartedOverlapping(e);
            overlap.StartedOverlapping(e);
            overlap.StartedOverlapping(e);
            overlap.StartedOverlapping(e);

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
            overlap.StoppedOverlapping(e);

            // Assert
            Assert.That(onStopCalled, Is.False);
        }

        public void StopedOverlappingDoesntTriggerCallbackIfCalledMultipleTimes()
        {
            var timesCalled = 0;

            // Arrange
            var overlap = new OverlapComponent(new Entity(), 32, 32, 0, 0, (o) => timesCalled++, null);

            var e = new Entity();
            var target = new OverlapComponent(e, 32, 32);
            overlap.StartedOverlapping(e);

            // Act
            overlap.StoppedOverlapping(e);
            overlap.StoppedOverlapping(e);
            overlap.StoppedOverlapping(e);
            overlap.StoppedOverlapping(e);

            // Assert
            Assert.That(timesCalled, Is.EqualTo(1));
        }
    }
}