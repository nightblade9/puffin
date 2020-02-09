using System;
using Moq;
using NUnit.Framework;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Core.Ecs.Systems;
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
            var hasSprite = e.Get<SpriteComponent>();
            Assert.That(hasSprite, Is.Not.Null);
            Assert.That(hasSprite.FileName, Is.EqualTo("moon.bmp"));
        }

        [Test]
        public void SpritesheetSetsSpriteAndFrameSize()
        {
            var e = new Entity().Spritesheet("player.png", 48, 32);
            var sprite = e.Get<SpriteComponent>();
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
            
            var label = e.Get<TextLabelComponent>();
            Assert.That(label, Is.Not.Null);
            Assert.That(label.Text, Is.EqualTo("hi!"));
        }

        public void MouseSetsMouseComponent()
        {
            var provider = new Mock<IMouseProvider>();
            DependencyInjection.Kernel.Bind<IMouseProvider>().ToConstant(provider.Object);

            var e = new Entity();
            Assert.That(e.Get<MouseComponent>(), Is.Null);

            e.Mouse(null, 32, 32);
            Assert.That(e.Get<MouseComponent>(), Is.Not.Null);
        }

        [Test]
        public void KeyboardSetsKeyboardComponent()
        {
            // Depends on default mapping for PuffinGame.
            var provider = new Mock<IKeyboardProvider>();
            DependencyInjection.Kernel.Bind<IKeyboardProvider>().ToConstant(provider.Object);

            var e = new Entity();
            Assert.That(e.Get<KeyboardComponent>(), Is.Null);

            e.Keyboard();
            Assert.That(e.Get<KeyboardComponent>(), Is.Not.Null);
        }

        [Test]
        public void FourWayMovementSetsFourWayMovementComponent()
        {
            // Depends on default mapping for PuffinGame.
            var provider = new Mock<IKeyboardProvider>();
            DependencyInjection.Kernel.Bind<IKeyboardProvider>().ToConstant(provider.Object);

            var e = new Entity();
            e.FourWayMovement(210);
            
            var actual = e.Get<FourWayMovementComponent>();
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.Speed, Is.EqualTo(210));
        }

        [Test]
        public void OverlapSetsAllProperties()
        {
            var numStarts = 0;
            var stopped = false;
            Action<Entity> onStart = (e) => numStarts++;
            Action<Entity> onStop = (e) => stopped = true;

            var o1 = new Entity().Overlap(10, 11).Get<OverlapComponent>();
            Assert.That(o1, Is.Not.Null);
            Assert.That(o1.Size.Item1, Is.EqualTo(10));
            Assert.That(o1.Size.Item2, Is.EqualTo(11));

            var o2 = new Entity().Overlap(12, 13, 14, 15).Get<OverlapComponent>();
            Assert.That(o2, Is.Not.Null);
            Assert.That(o2.Size.Item1, Is.EqualTo(12));
            Assert.That(o2.Size.Item2, Is.EqualTo(13));
            Assert.That(o2.Offset.Item1, Is.EqualTo(14));
            Assert.That(o2.Offset.Item2, Is.EqualTo(15));

            var o3 = new Entity().Overlap(16, 17, 18, 19, onStart).Get<OverlapComponent>();
            OverlapSystem.StartedOverlapping(o3, new Entity().Overlap(100, 100));

            Assert.That(o3, Is.Not.Null);
            Assert.That(o3.Size.Item1, Is.EqualTo(16));
            Assert.That(o3.Size.Item2, Is.EqualTo(17));
            Assert.That(o3.Offset.Item1, Is.EqualTo(18));
            Assert.That(o3.Offset.Item2, Is.EqualTo(19));
            Assert.That(numStarts, Is.EqualTo(1));

            var o4 = new Entity().Overlap(1, 7, 1, 9, onStart, onStop).Get<OverlapComponent>();
            var e = new Entity().Overlap(200, 50);
            OverlapSystem.StartedOverlapping(o4, e);
            OverlapSystem.StoppedOverlapping(o4, e);

            Assert.That(o4, Is.Not.Null);
            Assert.That(o4.Size.Item1, Is.EqualTo(1));
            Assert.That(o4.Size.Item2, Is.EqualTo(7));
            Assert.That(o4.Offset.Item1, Is.EqualTo(1));
            Assert.That(o4.Offset.Item2, Is.EqualTo(9));
            Assert.That(numStarts, Is.EqualTo(2));
            Assert.That(stopped, Is.True);

            var lastCalled = "";
            Action mouseOverlap = () => lastCalled = "start";
            Action mouseStopOverlap = () => lastCalled = "stop";

            var o5 = new Entity().Overlap(32, 32, -1, -1, mouseOverlap, mouseStopOverlap).Get<OverlapComponent>();
            Assert.That(o5, Is.Not.Null);
            Assert.That(o5.Size.Item1, Is.EqualTo(32));
            Assert.That(o5.Size.Item2, Is.EqualTo(32));
            Assert.That(o5.Offset.Item1, Is.EqualTo(-1));
            Assert.That(o5.Offset.Item2, Is.EqualTo(-1));
            Assert.That(o5.OnMouseEnter, Is.EqualTo(mouseOverlap));
            Assert.That(o5.OnMouseExit, Is.EqualTo(mouseStopOverlap));
        }

        [Test]
        public void AudioSetsAudioComponent()
        {
            var e = new Entity().Audio("blue-heron.ogg");
            
            var actual = e.Get<AudioComponent>();
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.FileName, Is.EqualTo("blue-heron.ogg"));
        }

        [Test]
        public void ColourSetsColourComponent()
        {
            var e = new Entity().Colour(0x88ff00, 64, 32);
            
            var actual = e.Get<ColourComponent>();
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.Colour, Is.EqualTo(0x88ff00));
            Assert.That(actual.Width, Is.EqualTo(64));
            Assert.That(actual.Height, Is.EqualTo(32));
        }
        
        [Test]
        public void CollideSetsCollisionComponent()
        {
            var e = new Entity().Collide(64, 32, true);

            var actual = e.Get<CollisionComponent>();
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.Width, Is.EqualTo(64));
            Assert.That(actual.Height, Is.EqualTo(32));
            Assert.That(actual.SlideOnCollide, Is.True);
        }

        [Test]
        public void CollideSetsOnCollisionCallback()
        {
            Action<Entity, string> expected = (e, s) => {};

            var e = new Entity().Collide(16, 8, expected);
            var actual = e.Get<CollisionComponent>();
            Assert.That(actual.onCollide, Is.EqualTo(expected));
        }

        [Test]
        public void VelocitySetsVelocity()
        {
            var e = new Entity().Velocity(100, 50);
            Assert.That(e.VelocityX, Is.EqualTo(100));
            Assert.That(e.VelocityY, Is.EqualTo(50));
        }
    }
}