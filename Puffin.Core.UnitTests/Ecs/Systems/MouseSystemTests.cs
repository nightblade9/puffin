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

            var eventBus = new EventBus();
            var callbackFired = false;
            var entity = new Entity().Move(20, 10).Mouse(32, 32, (x, y, clickType) => callbackFired = true);
            var system = new MouseSystem(eventBus, mouseProvider.Object);
            system.OnAddEntity(entity);
            mouseProvider.Setup(m => m.MouseCoordinates).Returns(new Tuple<int, int>(clickedX, clickedY));
            mouseProvider.Setup(m => m.IsButtonDown(ClickType.LeftClick)).Returns(true);

            // Act
            eventBus.Broadcast(EventBusSignal.MouseClicked, null);

            // Assert
            Assert.That(callbackFired, Is.False);
        }

        [Test]
        public void OnUpdateFiresCallbackIfClickIsInBoundsAndMatchesClickType()
        {
            // Arrange
            var mouseProvider = new Mock<IMouseProvider>();
            const int clickedX = 90;
            const int clickedY = 91;

            var eventBus = new EventBus();
            var callbackFired = false;
            var entity = new Entity().Move(77, 88).Mouse(32, 32, (x, y, clickType) => 
            {
                Assert.That(clickType, Is.EqualTo(ClickType.RightClick));
                Assert.That(x, Is.EqualTo(clickedX));
                Assert.That(y, Is.EqualTo(clickedY));
                callbackFired = true;
                return true;
            });
            mouseProvider.Setup(m => m.MouseCoordinates).Returns(new Tuple<int, int>(clickedX, clickedY));
            mouseProvider.Setup(m => m.IsButtonDown(ClickType.RightClick)).Returns(true);
            var system = new MouseSystem(eventBus, mouseProvider.Object);
            system.OnAddEntity(entity);

            // Act
            system.OnUpdate(TimeSpan.Zero);
            
            // Assert
            Assert.That(callbackFired, Is.True);
        }

        [Test]
        public void OnUpdateBroadcastsMouseReleasedEvent()
        {
            var isCalledBack = false;

            var eventBus = new EventBus();
            eventBus.Subscribe(EventBusSignal.MouseReleased, (a) => isCalledBack = true);

            var mouseProvider = new Mock<IMouseProvider>();
            mouseProvider.Setup(m => m.IsButtonDown(ClickType.LeftClick)).Returns(true);
            var system = new MouseSystem(eventBus, mouseProvider.Object);
            system.OnUpdate(TimeSpan.Zero);
            mouseProvider.Setup(m => m.IsButtonDown(ClickType.LeftClick)).Returns(false);
            
            // Act
            system.OnUpdate(TimeSpan.Zero);

            // Assert
            Assert.That(isCalledBack, Is.True);
        }

        [Test]
        public void OnUpdateBroadcastsMouseClickedIfEntityHandlersDontProcessEvent()
        {
            var isCalledBack = false;

            var eventBus = new EventBus();
            eventBus.Subscribe(EventBusSignal.MouseClicked, (a) => isCalledBack = true);

            var mouseProvider = new Mock<IMouseProvider>();
            mouseProvider.Setup(m => m.IsButtonDown(ClickType.LeftClick)).Returns(true);
            mouseProvider.Setup(m => m.MouseCoordinates).Returns(new Tuple<int, int>(30, 17));
            
            var system = new MouseSystem(eventBus, mouseProvider.Object);
            // False: did not process the event
            system.OnAddEntity(new Entity().Mouse(999, 999, (x, y, clickType) => false));
            
            // Act
            system.OnUpdate(TimeSpan.Zero);

            // Assert
            Assert.That(isCalledBack, Is.True);
        }

        [Test]
        public void OnUpdateDoesntBroadcastMouseClickedIfEntityHandlersProcessEvent()
        {
            var isCalledBack = false;
            var globalCallback = false;

            var eventBus = new EventBus();
            eventBus.Subscribe(EventBusSignal.MouseClicked, (a) => globalCallback = true);

            var mouseProvider = new Mock<IMouseProvider>();
            mouseProvider.Setup(m => m.IsButtonDown(ClickType.LeftClick)).Returns(true);
            mouseProvider.Setup(m => m.MouseCoordinates).Returns(new Tuple<int, int>(30, 17));
            
            var system = new MouseSystem(eventBus, mouseProvider.Object);
            // False: did not process the event
            system.OnAddEntity(new Entity().Mouse(999, 999, (x, y, clickType) =>
            {
                isCalledBack = true;
                return true;
            }));
            
            // Act
            system.OnUpdate(TimeSpan.Zero);

            // Assert
            Assert.That(isCalledBack, Is.True);
            Assert.That(globalCallback, Is.False);
        }

        [Test]
        public void RemoveEntityRemovesEntity()
        {
            // Arrange
            var mouseProvider = new Mock<IMouseProvider>();

            var eventBus = new EventBus();
            var callbackFired = false;
            var entity = new Entity().Move(77, 88).Mouse(48, 48, (x, y, clickType) => callbackFired = true);
            mouseProvider.Setup(m => m.MouseCoordinates).Returns(new Tuple<int, int>(90, 90));
            var system = new MouseSystem(eventBus, mouseProvider.Object);
            system.OnAddEntity(entity);

            // Act
            system.OnRemoveEntity(entity);
            eventBus.Broadcast(EventBusSignal.MouseClicked, null);
            
            // Assert: removed entities don't trigger callbacks.
            Assert.That(callbackFired, Is.False);
        }
    }
}