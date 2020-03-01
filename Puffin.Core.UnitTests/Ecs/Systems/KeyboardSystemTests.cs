using System;
using Moq;
using Ninject;
using NUnit.Framework;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Systems;
using Puffin.Core.Events;
using Puffin.Core.IO;

namespace Puffin.Core.UnitTests.Ecs
{
    [TestFixture]
    public class KeyboardSystemTests
    {
        
        [Test]
        public void OnActionPressedInvokesCallbackOnEntitiesWithKeyboardComponents()
        {
            // Arrange

            // Not used directly, just needs a binding
            var provider = new Mock<IKeyboardProvider>();

            var eventBus = new EventBus();
            var system = new KeyboardSystem(eventBus, provider.Object);

            var isCalled = false;
            var entity = new Entity().Keyboard((e) => isCalled = true);
            system.OnAddEntity(entity);

            // Act
            eventBus.Broadcast(EventBusSignal.ActionPressed, CustomActions.Home);

            // Assert
            Assert.That(isCalled, Is.True);
        }

        [Test]
        public void OnActionReleasedInvokesCallbackOnEntitiesWithKeyboardComponents()
        {
            // press, then release/assert
            // Arrange

            // Not used directly, just needs a binding
            var provider = new Mock<IKeyboardProvider>();

            var eventBus = new EventBus();
            var system = new KeyboardSystem(eventBus, provider.Object);

            var isCalled = false;
            var entity = new Entity().Keyboard(null, (e) => isCalled = true);
            system.OnAddEntity(entity);

            // Act
            eventBus.Broadcast(EventBusSignal.ActionReleased, CustomActions.Home);

            // Assert
            Assert.That(isCalled, Is.True);
        }

        [Test]
        public void OnRemoveEntityRemovesEntity()
        {
            // Same as press; just remove entity and then fire.
            // Arrange

            // Not used directly, just needs a binding
            var provider = new Mock<IKeyboardProvider>();

            var eventBus = new EventBus();
            var system = new KeyboardSystem(eventBus, provider.Object);

            var isCalled = false;
            var entity = new Entity().Keyboard((e) => isCalled = true);
            system.OnAddEntity(entity);
            system.OnRemoveEntity(entity);

            // Act
            eventBus.Broadcast(EventBusSignal.ActionPressed, CustomActions.Home);

            // Assert
            Assert.That(isCalled, Is.False);
        }

        [Test]
        public void OnActionDownFiresWhileKeyIsPressed()
        {
            // Arrange
            var eventBus = new EventBus();
            var provider = new Mock<IKeyboardProvider>();
            var system = new KeyboardSystem(eventBus, provider.Object);

            var numCalls = 0;
            var entity = new Entity().Keyboard(onActionDown: (e) => numCalls++);
            system.OnAddEntity(entity);

            // Act/Assert
            // Initially false
            Assert.That(numCalls, Is.EqualTo(0));

            // When we call OnActionPressed, gets invoked
            eventBus.Broadcast(EventBusSignal.ActionPressed, CustomActions.Next);
            system.OnUpdate(TimeSpan.Zero);
            Assert.That(numCalls, Is.EqualTo(1));
            system.OnUpdate(TimeSpan.Zero);
            Assert.That(numCalls, Is.EqualTo(2));

            // When we call OnActionReleased, no longer invoked
            eventBus.Broadcast(EventBusSignal.ActionReleased, CustomActions.Next);
            system.OnUpdate(TimeSpan.Zero);

            Assert.That(numCalls, Is.EqualTo(2));
        }

        enum CustomActions
        {
            Home,
            Back,
            Next,
        }
    }
}