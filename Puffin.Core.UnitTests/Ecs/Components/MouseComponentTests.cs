using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Puffin.Core.Ecs;
using Puffin.Core.IO;

namespace Puffin.Core.UnitTests.Ecs
{
    [TestFixture]
    public class MouseComponentTests
    {
        [TearDown]
        public void ResetDependencyInjectionBindings()
        {
            DependencyInjection.Reset();
        }

        [TestCase(-10, 3)]
        [TestCase(31, 3)]
        [TestCase(100, -5)]

        [TestCase(17, 17)]
        [TestCase(64, 22)]

        [TestCase(-9, 93)]
        [TestCase(28, 53)]
        [TestCase(66, 61)]
        public void OnClickCallbackDoesNotFireIfClickIsOutOfBounds(int clickedX, int clickedY)
        {
            var mouseProvider = new Mock<IMouseProvider>();
            DependencyInjection.Kernel.Bind<IMouseProvider>().ToConstant(mouseProvider.Object);

            var eventBus = new EventBus();
            var entity = new Entity().Move(20, 10);
            var callbackFired = false;
            var component = new MouseComponent(entity, () => callbackFired = true, 32, 32);

            mouseProvider.Setup(m => m.MouseCoordinates).Returns(new Tuple<int, int>(clickedX, clickedY));
            eventBus.Broadcast(EventBusSignal.MouseClicked, null);
            Assert.That(callbackFired, Is.False);
        }

        [Test]
        public void OnClickCallbackFiresIfClickIsInBounds()
        {
            var mouseProvider = new Mock<IMouseProvider>();
            DependencyInjection.Kernel.Bind<IMouseProvider>().ToConstant(mouseProvider.Object);

            var eventBus = new EventBus();
            var entity = new Entity().Move(77, 88);
            var callbackFired = false;
            var component = new MouseComponent(entity, () => callbackFired = true, 32, 32);

            mouseProvider.Setup(m => m.MouseCoordinates).Returns(new Tuple<int, int>(90, 90));
            eventBus.Broadcast(EventBusSignal.MouseClicked, null);
            Assert.That(callbackFired, Is.True);
        }
    }
}