using System;
using Moq;
using Ninject;
using NUnit.Framework;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Systems;
using Puffin.Core.IO;

namespace Puffin.Core.UnitTests.Ecs
{
    [TestFixture]
    public class KeyboardSystemTests
    {
        [TearDown]
        public void ResetDependencyInjectionBindings()
        {
            DependencyInjection.Reset();
        }

        [Test]
        public void OnActionPressedInvokesCallbackOnEntitiesWithKeyboardComponents()
        {
            // Arrange

            // Not used directly, just needs a binding
            var provider = new Mock<IKeyboardProvider>();
            DependencyInjection.Kernel.Bind<IKeyboardProvider>().ToConstant(provider.Object);

            var eventBus = new EventBus();
            var system = new KeyboardSystem();

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
            DependencyInjection.Kernel.Bind<IKeyboardProvider>().ToConstant(provider.Object);

            var eventBus = new EventBus();
            var system = new KeyboardSystem();

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
            DependencyInjection.Kernel.Bind<IKeyboardProvider>().ToConstant(provider.Object);

            var eventBus = new EventBus();
            var system = new KeyboardSystem();

            var isCalled = false;
            var entity = new Entity().Keyboard((e) => isCalled = true);
            system.OnAddEntity(entity);
            system.OnRemoveEntity(entity);

            // Act
            eventBus.Broadcast(EventBusSignal.ActionPressed, CustomActions.Home);

            // Assert
            Assert.That(isCalled, Is.False);
        }

        enum CustomActions
        {
            Home,
            Back,
            Next,
        }
    }
}