using System;
using Moq;
using NUnit.Framework;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Systems;
using Puffin.Core.Events;
using Puffin.Core.IO;

namespace Puffin.Core.UnitTests.Ecs
{
    [TestFixture]
    public class MouseSystemTests
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
            // Arrange
            var mouseProvider = new Mock<IMouseProvider>();
            DependencyInjection.Kernel.Bind<IMouseProvider>().ToConstant(mouseProvider.Object);

            var eventBus = new EventBus();
            var callbackFired = false;
            var entity = new Entity().Move(20, 10).Mouse(32, 32, () => callbackFired = true);
            var system = new MouseSystem();
            system.OnAddEntity(entity);
            mouseProvider.Setup(m => m.MouseCoordinates).Returns(new Tuple<int, int>(clickedX, clickedY));

            // Act
            eventBus.Broadcast(EventBusSignal.MouseClicked, null);

            // Assert
            Assert.That(callbackFired, Is.False);
        }

        [Test]
        public void OnClickCallbackFiresIfClickIsInBounds()
        {
            // Arrange
            var mouseProvider = new Mock<IMouseProvider>();
            DependencyInjection.Kernel.Bind<IMouseProvider>().ToConstant(mouseProvider.Object);

            var eventBus = new EventBus();
            var callbackFired = false;
            var entity = new Entity().Move(77, 88).Mouse(32, 32, () => callbackFired = true);
            mouseProvider.Setup(m => m.MouseCoordinates).Returns(new Tuple<int, int>(90, 90));
            var system = new MouseSystem();
            system.OnAddEntity(entity);

            // Act
            eventBus.Broadcast(EventBusSignal.MouseClicked, null);
            
            // Assert
            Assert.That(callbackFired, Is.True);
        }

        [Test]
        public void RemoveEntityRemovesEntity()
        {
            // Arrange
            var mouseProvider = new Mock<IMouseProvider>();
            DependencyInjection.Kernel.Bind<IMouseProvider>().ToConstant(mouseProvider.Object);

            var eventBus = new EventBus();
            var callbackFired = false;
            var entity = new Entity().Move(77, 88).Mouse(32, 32, () => callbackFired = true);
            mouseProvider.Setup(m => m.MouseCoordinates).Returns(new Tuple<int, int>(90, 90));
            var system = new MouseSystem();
            system.OnAddEntity(entity);

            // Act
            system.OnRemoveEntity(entity);
            eventBus.Broadcast(EventBusSignal.MouseClicked, null);
            
            // Assert: removed entities don't trigger callbacks.
            Assert.That(callbackFired, Is.False);
        }
    }
}