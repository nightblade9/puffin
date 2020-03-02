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
                
        
        [Test]
        public void OnUpdateCallsOnUpdateOnFourWayComponents()
        {
            // Arrange
            const int speed = 100;
            var elapsed = TimeSpan.FromSeconds(2);

            // Depends on default mapping for PuffinGame.
            // Arrange
            var provider = new Mock<IKeyboardProvider>();
            provider.Setup(p => p.IsActionDown(PuffinAction.Up)).Returns(true);
            provider.Setup(p => p.IsActionDown(PuffinAction.Right)).Returns(true);

            var e1 = new Entity().FourWayMovement(100);
            var e2 = new Entity().Sprite("flower.png");
            
            var scene = new Scene();
            scene.Initialize(new ISystem[] { new Mock<DrawingSystem>().Object }, new Mock<IMouseProvider>().Object, provider.Object);
            scene.Add(e1);
            scene.Add(e2);

            var system = new MovementSystem(scene);
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
            var scene = new Scene();
            var provider = new Mock<IKeyboardProvider>();
            provider.Setup(p => p.IsActionDown(PuffinAction.Up)).Returns(true);
            provider.Setup(p => p.IsActionDown(PuffinAction.Right)).Returns(true);

            scene.Initialize(new ISystem[] { new DrawingSystem() }, new Mock<IMouseProvider>().Object, new Mock<IKeyboardProvider>().Object);
            var e = new Entity().FourWayMovement(100);
            scene.Add(e);
            var system = new MovementSystem(new Scene());
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

        [TestCase(-1, 0)]
        [TestCase(1, 0)]
        [TestCase(0, -1)]
        [TestCase(0, 1)]
        // Dual axes, shortest resolves first
        [TestCase(1, 0.5f)]
        [TestCase(1, -0.5f)]
        [TestCase(-1, 0.5f)]
        [TestCase(-1, -0.5f)]
        [TestCase(0.5f, 1)]
        [TestCase(0.5f, -1)]
        [TestCase(-0.5f, 1)]
        [TestCase(-0.5f, -1)]
        public void ProcessMovementCollidesAndStopsEntityWithTile(float xDirection, float yDirection)
        {
            // There's nothing special about entity/tile collision, but we test thoroughly anyway,
            // because unit tests are cheap. This is a copy of the entity/entity test for axis
            // of collision / onCollide reporting from below.
            // Unlike the other test (box moves into drone), we flip (drone moves into box/cactus).
            Entity actualEntity = null;
            (int boxWidth, int boxHeight) = (48, 48);
            (int droneWidth, int dronerHeight) = (32, 32);
            (int xVelocity, int yVelocity) = ((int)(xDirection * 10), (int)(yDirection * 10));

            // Arrange
            var scene = new Scene();
            var tilemap = new TileMap(10, 10, "desert.png", 25, 25);
            tilemap.Define("Cactus", 0, 0, true);
            // Cactus at (75, 100) (pixels)
            tilemap.Set(3, 4, "Cactus");
            scene.Add(tilemap);

            // Note that DRONE moves into CACTUS.
            var drone = new Entity().Collide(droneWidth, dronerHeight, (e, s) => {
                actualEntity = e;
            })
            .Move(100 - xVelocity, 100 - yVelocity)
            .Velocity(xVelocity, yVelocity);

            scene.Add(drone);

            var system = new MovementSystem(scene);
            system.OnAddEntity(drone);

            // Act
            system.OnUpdate(TimeSpan.FromSeconds(1.1));

            // Assert
            Assert.That(actualEntity, Is.Not.EqualTo(drone)); // It's CACTUS!
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ProcessMovementCollidesAndStopsEntityWithEntity(bool slideOnCollide)
        {
            var scene = new Scene();

            var keyboardProvider = new Mock<IKeyboardProvider>();
            keyboardProvider.Setup(p => p.IsActionDown(PuffinAction.Down)).Returns(true);
            keyboardProvider.Setup(p => p.IsActionDown(PuffinAction.Right)).Returns(true);
            
            var system = new MovementSystem(scene);
            scene.Initialize(new ISystem[] { new DrawingSystem(), system }, null, keyboardProvider.Object);

            var player = new Entity().FourWayMovement(100).Collide(30, 30, slideOnCollide);

            scene.Add(player);
            scene.Add(new Entity().Collide(20, 20).Move(50, 40));

            // Act
            system.OnUpdate(TimeSpan.FromSeconds(0.1));
            system.OnUpdate(TimeSpan.FromSeconds(0.1));
            system.OnUpdate(TimeSpan.FromSeconds(0.1));
            system.OnUpdate(TimeSpan.FromSeconds(0.1));
            system.OnUpdate(TimeSpan.FromSeconds(0.1));

            // Assert
            if (slideOnCollide)
            {
                // Move more so it slides
                Assert.That(player.X, Is.EqualTo(20));
                Assert.That(player.Y, Is.EqualTo(40));
            }
            else
            {
                Assert.That(player.X, Is.EqualTo(20));
                Assert.That(player.Y, Is.EqualTo(20));
            }
        }

        public void ProcessMovementCollidesAndTakesIntoAccountCollisionOffsets(bool slideOnCollide)
        {
            var scene = new Scene();

            var keyboardProvider = new Mock<IKeyboardProvider>();
            keyboardProvider.Setup(p => p.IsActionDown(PuffinAction.Down)).Returns(true);
            keyboardProvider.Setup(p => p.IsActionDown(PuffinAction.Right)).Returns(true);
            
            var system = new MovementSystem(scene);
            scene.Initialize(new ISystem[] { new DrawingSystem(), system }, null, keyboardProvider.Object);

            var player = new Entity().FourWayMovement(100).Collide(40, 40, slideOnCollide);

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
        public void ProcessMovementCollidesAndResolvesWithCollisionOffsets()
        {
            var scene = new Scene();

            var keyboardProvider = new Mock<IKeyboardProvider>();
            keyboardProvider.Setup(p => p.IsActionDown(PuffinAction.Down)).Returns(true);
            keyboardProvider.Setup(p => p.IsActionDown(PuffinAction.Right)).Returns(true);
            
            var system = new MovementSystem(scene);
            scene.Initialize(new ISystem[] { new DrawingSystem(), system }, null, keyboardProvider.Object);

            var player = new Entity().FourWayMovement(100).Collide(30, 30, true, -10, -10);

            scene.Add(player);
            scene.Add(new Entity().Collide(20, 20, false, 15, 15).Move(50, 40));

            // Act
            system.OnUpdate(TimeSpan.FromSeconds(0.1));
            system.OnUpdate(TimeSpan.FromSeconds(0.1));
            system.OnUpdate(TimeSpan.FromSeconds(0.1));
            system.OnUpdate(TimeSpan.FromSeconds(0.1));
            system.OnUpdate(TimeSpan.FromSeconds(0.1));

            // Assert
            Assert.That(player.X, Is.EqualTo(40));
            Assert.That(player.Y, Is.EqualTo(40));
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
            keyboardProvider.Setup(p => p.IsActionDown(PuffinAction.Up)).Returns(true);
            keyboardProvider.Setup(p => p.IsActionDown(PuffinAction.Left)).Returns(true);
            
            var scene = new Scene();
            var system = new MovementSystem(scene);
            var drawingSystem = new DrawingSystem(new Mock<IDrawingSurface>().Object);

            scene.Initialize(new ISystem[] { drawingSystem, system }, new Mock<IMouseProvider>().Object, keyboardProvider.Object);
            scene.Add(tileMap);
            
            var player = new Entity().Collide(32, 32, true)
                .FourWayMovement(100)
                .Move(40, 32); // just left of the top-left non-wall tile
            
            scene.Add(player);
            
            // Act
            scene.OnUpdate(TimeSpan.FromSeconds(1));

            // Assert: player didn't move except into the corner. Faulty collision sees him resolve from the top wall and move into the left wall.
            Assert.That(player.X, Is.EqualTo(40));
            Assert.That(player.Y, Is.EqualTo(32));
        }
        
        [TestCase(-1, 0, "X")]
        [TestCase(1, 0, "X")]
        [TestCase(0, -1, "Y")]
        [TestCase(0, 1, "Y")]
        // Dual axes, shortest resolves first
        [TestCase(1, 0.5f, "X")]
        [TestCase(1, -0.5f, "X")]
        [TestCase(-1, 0.5f, "X")]
        [TestCase(-1, -0.5f, "X")]
        [TestCase(0.5f, 1, "Y")]
        [TestCase(0.5f, -1, "Y")]
        [TestCase(-0.5f, 1, "Y")]
        [TestCase(-0.5f, -1, "Y")]
        public void OnUpdateTriggersCollisionAndPassesInCorrectAxis(float xDirection, float yDirection, string expectedAxis)
        {
            Entity actualEntity = null;
            string actualAxis = "";
            (int boxWidth, int boxHeight) = (48, 48);
            (int droneWidth, int dronerHeight) = (32, 32);
            (int xVelocity, int yVelocity) = ((int)(xDirection * 10), (int)(yDirection * 10));

            // Arrange. Note that BOX moves into DRONE.
            var scene = new Scene();
            var box = new Entity().Move(100, 100).Velocity(xVelocity, yVelocity)
            .Collide(boxWidth, boxHeight, (e, s) => {
                actualEntity = e;
                actualAxis = s;
            });

            var drone = new Entity().Collide(droneWidth, dronerHeight, true).Move((int)(box.X - xVelocity), (int)(box.Y - yVelocity));

            var system = new MovementSystem(scene);
            system.OnAddEntity(box);
            system.OnAddEntity(drone);

            // Act
            system.OnUpdate(TimeSpan.FromSeconds(1.1));

            // Assert
            Assert.That(actualEntity, Is.EqualTo(drone));
            Assert.That(actualAxis, Is.EqualTo(expectedAxis));
        }
    }
}