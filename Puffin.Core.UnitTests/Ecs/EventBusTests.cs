using System;
using NUnit.Framework;
using Puffin.Core.Ecs;

namespace Puffin.Core.UnitTests.Ecs
{
    [TestFixture]
    public class EventBusTests
    {
        [Test]
        public void LatestInstanceGetsLatestInstance()
        {
            var b1 = new EventBus();
            var b2 = new EventBus();

            Assert.That(EventBus.LatestInstance, Is.EqualTo(b2));
        }

        [Test]
        public void BroadcastTriggersSubscribedCallbacksWithData()
        {
            // Arrange
            var bus = new EventBus();
            var wasCalled = new bool[2];
            bus.Subscribe("call it", (data) => wasCalled[0] = true);
            bus.Subscribe("call it", (data) => wasCalled[1] = true);

            // Act
            bus.Broadcast("call it");

            // Assert
            Assert.That(wasCalled[0], Is.True);
            Assert.That(wasCalled[1], Is.True);
        }

        [Test]
        public void UnsubscribeRemovesCallback()
        {
            // Arrange
            var bus = new EventBus();
            var wasCalled = new bool[2];
            Action<object> setSecondValue = (data) => wasCalled[1] = true;

            bus.Subscribe("call it", (data) => wasCalled[0] = true);
            
            bus.Subscribe("call it", setSecondValue);
            bus.Unsubscribe("call it", setSecondValue);

            // Act
            bus.Broadcast("call it");

            // Assert
            Assert.That(wasCalled[0], Is.True);
            Assert.That(wasCalled[1], Is.False);
        }
    }
}