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
        public void SpriteSetsSpriteComponent()
        {
            var e = new Entity().Sprite("moon.bmp");
            var hasSprite = e.GetIfHas<SpriteComponent>();
            Assert.That(hasSprite, Is.Not.Null);
            Assert.That(hasSprite.FileName, Is.EqualTo("moon.bmp"));
        }

        [Test]
        public void SpritesheetSetsSpriteAndFrameSize()
        {
            var e = new Entity().Spritesheet("player.png", 48, 32);
            var sprite = e.GetIfHas<SpriteComponent>();
            Assert.That(sprite, Is.Not.Null);
            Assert.That(sprite.FileName, Is.EqualTo("player.png"));
            Assert.That(sprite.FrameWidth, Is.EqualTo(48));
            Assert.That(sprite.FrameHeight, Is.EqualTo(32));
        }

        [Test]
        public void LabelSetsTextLabelAndText()
        {
            var e = new Entity();
            e.Label("hi!");
            
            var label = e.GetIfHas<TextLabelComponent>();
            Assert.That(label, Is.Not.Null);
            Assert.That(label.Text, Is.EqualTo("hi!"));
        }
    }
}