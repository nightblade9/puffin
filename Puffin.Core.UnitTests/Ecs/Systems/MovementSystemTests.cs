using System;
using Moq;
using NUnit.Framework;
using Puffin.Core.Drawing;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Systems;
using Puffin.Core.IO;
using Puffin.Core.Tiles;

namespace Puffin.Core.UnitTests
{
    [TestFixture]
    public class MovementSystemTests
    {
        [TearDown]
        public void ResetDependencyInjectionBindings()
        {
            DependencyInjection.Reset();
        }
        
        
        [Test]
        public void OnUpdateCallsOnUpdateOnFourWayComponents()
        {
            // Arrange
            const int speed = 100;
            var elapsed = TimeSpan.FromSeconds(2);

            // Depends on default mapping for PuffinGame.
            // Arrange
            var provider = new Mock<IKeyboardProvider>();
            DependencyInjection.Kernel.Bind<IKeyboardProvider>().ToConstant(provider.Object);
            provider.Setup(p => p.IsActionDown(PuffinAction.Up)).Returns(true);
            provider.Setup(p => p.IsActionDown(PuffinAction.Right)).Returns(true);

            var e1 = new Entity().FourWayMovement(100);
            var e2 = new Entity().Sprite("flower.png");

            var system = new MovementSystem();
            system.OnAddEntity(e1);
            system.OnAddEntity(e2);

            // Act
            system.OnUpdate(TimeSpan.FromSeconds(1));

            // Assert
            // e1 has a movement component so it moved
            Assert.That(e1.X, Is.EqualTo(speed));
            Assert.That(e1.Y, Is.EqualTo(-speed));
            
            // e2 doesn't have a movement component so it's at the start position
            Assert.That(e2.X, Is.EqualTo(0));
            Assert.That(e2.Y, Is.EqualTo(0));

        }

        [Test]
        public void OnRemoveRemovesEntity()
        {
            var provider = new Mock<IKeyboardProvider>();
            DependencyInjection.Kernel.Bind<IKeyboardProvider>().ToConstant(provider.Object);
            provider.Setup(p => p.IsActionDown(PuffinAction.Up)).Returns(true);
            provider.Setup(p => p.IsActionDown(PuffinAction.Right)).Returns(true);

            var e = new Entity().FourWayMovement(100);
            var system = new MovementSystem();
            system.OnAddEntity(e);
            system.OnUpdate(TimeSpan.FromSeconds(1));
            var expectedPosition = new Tuple<float, float>(e.X, e.Y);

            // Act
            system.OnRemoveEntity(e);
            system.OnUpdate(TimeSpan.FromSeconds(10));

            // Assert: removed entities don't move
            Assert.That(e.X, Is.EqualTo(expectedPosition.Item1));
            Assert.That(e.Y, Is.EqualTo(expectedPosition.Item2));
        }

        [Test]
        public void ProcessMovementCollidesAndStopsEntityWithTile()
        {
            
        }

        [TestCase(false)]
        [TestCase(true)]
        public void ProcessMovementCollidesAndStopsEntityWithEntity(bool slideOnCollide)
        {
            var scene = new Scene();

            var keyboardProvider = new Mock<IKeyboardProvider>();
            DependencyInjection.Kernel.Bind<IKeyboardProvider>().ToConstant(keyboardProvider.Object);
            keyboardProvider.Setup(p => p.IsActionDown(PuffinAction.Down)).Returns(true);
            keyboardProvider.Setup(p => p.IsActionDown(PuffinAction.Right)).Returns(true);
            
            var system = new MovementSystem();
            scene.Initialize(new ISystem[] { new DrawingSystem(), system }, null, keyboardProvider.Object);

            var player = new Entity().FourWayMovement(100, slideOnCollide).Collide(40, 40);

            scene.Add(player);
            scene.Add(new Entity().Collide(50, 50).Move(25, 50));

            // Act
            system.OnUpdate(TimeSpan.FromSeconds(0.2));

            // Assert
            // Unobstructed, should reach (100, 100).
            if (slideOnCollide)
            {
                // Move more so it slides
                Assert.That(player.X, Is.EqualTo(20));
                Assert.That(player.Y, Is.EqualTo(0));
            }
            else
            {
                Assert.That(player.X, Is.EqualTo(10));
                Assert.That(player.Y, Is.EqualTo(10));
            }
        }

        [Test]
        public void OnUpdateHandlesSlidingWithMultiCollisionResolutions()
        {
            // You're sandwiched against the corner wall. Moving up/left shouldn't
            // resolve one collision and offset you into the wall by mistake.
            // See: https://twitter.com/nightblade99/status/1221945460157485061

            // Arrange
            var tileMap = new TileMap(7, 7, "tiles.png", 32, 32);

            tileMap.Define("Wall", 0, 0, true);
            for (var i = 0; i < 7; i++)
            {
                tileMap[i, 0] = "Wall";
                tileMap[i, 6] = "Wall";
                tileMap[0, i] = "Wall";
                tileMap[6, i] = "Wall";
            }

            var keyboardProvider = new Mock<IKeyboardProvider>();
            DependencyInjection.Kernel.Bind<IKeyboardProvider>().ToConstant(keyboardProvider.Object);
            keyboardProvider.Setup(p => p.IsActionDown(PuffinAction.Up)).Returns(true);
            keyboardProvider.Setup(p => p.IsActionDown(PuffinAction.Left)).Returns(true);
            
            var system = new MovementSystem();
            var drawingSystem = new DrawingSystem(new Mock<IDrawingSurface>().Object);
            
            var scene = new Scene();
            scene.Initialize(new ISystem[] { drawingSystem, system }, null, keyboardProvider.Object);
            scene.Add(tileMap);

            var player = new Entity().Collide(32, 32)
                .FourWayMovement(100, true)
                .Move(40, 32); // just left of the top-left non-wall tile
            
            scene.Add(player);
            
            // Act
            scene.OnUpdate(TimeSpan.FromSeconds(1));

            // Assert: player didn't move. Faulty collision sees him resolve from the top
            // wall and move into the left wall.
            Assert.That(player.X, Is.EqualTo(32));
            Assert.That(player.Y, Is.EqualTo(32));
        }
    }
}