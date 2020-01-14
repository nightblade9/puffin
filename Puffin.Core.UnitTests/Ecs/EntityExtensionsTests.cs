using NUnit.Framework;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;

namespace Puffin.Core.UnitTests.Ecs
{
    [TestFixture]
    public class EntityExtensionsTests
    {
        [Test]
        public void MoveSetsEntityCoordinatesAndBroadcastsEvent()
        {
            new EventBus();
            
            // Arrange
            var e = new Entity();
            var callbackCalled = false;
            new EventBus().Subscribe(EventBusSignal.EntityPositionChanged, (data) => callbackCalled = (data == e));

            // Act
            e.Move(1, 2);
            e.Move(200, 140);

            // Assert
            Assert.That(e.X, Is.EqualTo(200));
            Assert.That(e.Y, Is.EqualTo(140));
            Assert.That(callbackCalled, Is.True);
        }

        [Test]
        public void ImageSetsSpriteComponent()
        {
            var e = new Entity().Image("moon.bmp");
            var hasSprite = e.GetIfHas<SpriteComponent>();
            Assert.That(hasSprite, Is.Not.Null);
            Assert.That(hasSprite.FileName, Is.EqualTo("moon.bmp"));
        }
    }
}