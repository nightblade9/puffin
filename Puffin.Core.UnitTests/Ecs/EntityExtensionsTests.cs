using Moq;
using NUnit.Framework;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Core.IO;

namespace Puffin.Core.UnitTests.Ecs
{
    [TestFixture]
    public class EntityExtensionsTests
    {
        [TearDown]
        public void ResetDependencyInjectionBindings()
        {
            DependencyInjection.Reset();
        }

        [Test]
        public void MoveSetsEntityCoordinates()
        {
            new EventBus();
            
            // Arrange
            var e = new Entity();

            // Act
            e.Move(1, 2);
            e.Move(200, 140);

            // Assert
            Assert.That(e.X, Is.EqualTo(200));
            Assert.That(e.Y, Is.EqualTo(140));
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

        public void MouseSetsMouseComponent()
        {
            var provider = new Mock<IMouseProvider>();
            DependencyInjection.Kernel.Bind<IMouseProvider>().ToConstant(provider.Object);

            var e = new Entity();
            Assert.That(e.GetIfHas<MouseComponent>(), Is.Null);

            e.Mouse(null, 32, 32);
            Assert.That(e.GetIfHas<MouseComponent>(), Is.Not.Null);
        }

        [Test]
        public void KeyboardSetsKeyboardComponent()
        {
            // Depends on default mapping for PuffinGame.
            var provider = new Mock<IKeyboardProvider>();
            DependencyInjection.Kernel.Bind<IKeyboardProvider>().ToConstant(provider.Object);

            var e = new Entity();
            Assert.That(e.GetIfHas<KeyboardComponent>(), Is.Null);

            e.Keyboard();
            Assert.That(e.GetIfHas<KeyboardComponent>(), Is.Not.Null);
        }

        [Test]
        public void FourWayMovementAddsFourWayMovementComponent()
        {
            // Depends on default mapping for PuffinGame.
            var provider = new Mock<IKeyboardProvider>();
            DependencyInjection.Kernel.Bind<IKeyboardProvider>().ToConstant(provider.Object);

            var e = new Entity();
            e.FourWayMovement(210);
            
            var actual = e.GetIfHas<FourWayMovementComponent>();
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.Speed, Is.EqualTo(210));
        }
    }
}