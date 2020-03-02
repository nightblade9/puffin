using System;
using NUnit.Framework;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Core.Events;

namespace Puffin.Core.UnitTests.Ecs.Components
{
    [TestFixture]
    public class SpriteComponentTests
    {
        [TestCase(-193)]
        [TestCase(-1)]
        public void FrameIndexSetterThrowsIfIndexIsNegative(int index)
        {
            var component = new SpriteComponent(new Entity(), "galaxy.png", 16, 16);
            Assert.Throws<ArgumentException>(() => component.FrameIndex = index);
        }

        [Test]
        public void FrameIndexSetterThrowsIfFrameWidthOrHeightArentSet()
        {
            var scene = new Scene();
            var entity = new Entity();
            scene.Add(entity);
            var component = new SpriteComponent(entity, "galaxy.png");
            Assert.Throws<ArgumentException>(() => component.FrameIndex = 2323);
        }

        [Test]
        public void FrameIndexChangeBroadcastsFrameIndexChangedEvent()
        {
            // Arrange
            var scene = new Scene();
            var entity = new Entity();
            var component = new SpriteComponent(entity, "galaxy.png", 32, 32);
            entity.Set(component);
            scene.Add(entity);

            var called = false;
            scene.EventBus.Subscribe(EventBusSignal.SpriteSheetFrameIndexChanged, (data) => called = true);

            // Act
            component.FrameIndex = 3;

            // Assert
            Assert.That(called, Is.True);
        }
    }
}