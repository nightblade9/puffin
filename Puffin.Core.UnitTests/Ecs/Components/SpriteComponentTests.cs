using System;
using NUnit.Framework;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;

namespace Puffin.Core.UnitTests.Ecs.Components
{
    [TestFixture]
    public class SpriteComponentTests
    {
        [TestCase(-193)]
        [TestCase(-1)]
        public void FrameIndexSetterThrowsIfIndexIsNegative(int index)
        {
            var component = new SpriteComponent("galaxy.png", 16, 16);
            Assert.Throws<ArgumentException>(() => component.FrameIndex = index);
        }

        public void FrameIndexSetterThrowsIfFrameWidthOrHeightArentSet()
        {
            var component = new SpriteComponent("galaxy.png");
            Assert.Throws<ArgumentException>(() => component.FrameIndex = 2323);
        }

        [Test]
        public void FrameIndexChangeBroadcastsFrameIndexChangedEvent()
        {
            // Arrange
            var component = new SpriteComponent("galaxy.png", 32, 32);
            var called = false;
            new EventBus().Subscribe(EventBusSignal.SpriteSheetFrameIndexChanged, (data) => called = true);

            // Act
            component.FrameIndex = 3;

            // Assert
            Assert.That(called, Is.True);
        }
    }
}