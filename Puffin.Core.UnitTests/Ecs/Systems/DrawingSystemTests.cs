using System;
using Moq;
using NUnit.Framework;
using Puffin.Core.Drawing;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Systems;
using Puffin.Core.Tiles;

namespace Puffin.Core.UnitTests
{
    [TestFixture]
    public class DrawingSystemTests
    {
        [Test]
        public void OnDrawCallsDrawAll()
        {
            // Arrange
            var drawingSurface = new Mock<IDrawingSurface>();
            var system = new DrawingSystem(drawingSurface.Object);

            // Act
            system.OnDraw(TimeSpan.Zero, 0, "");

            // Assert
            drawingSurface.Verify(d => d.DrawAll(0x000000, "", true), Times.Once());
        }

        [Test]
        public void OnRemoveRemovesEntityFromDrawingSurface()
        {
            var entity = new Entity();
            var drawingSurface = new Mock<IDrawingSurface>();
            var drawingSystem = new DrawingSystem(drawingSurface.Object);
            drawingSystem.OnAddEntity(entity);

            // Act
            drawingSystem.OnRemoveEntity(entity);

            // Assert
            drawingSurface.Verify(s => s.RemoveEntity(entity), Times.Once());
        }

        [Test]
        public void OnAddTileMapAddsItToDrawingSurface()
        {
            // Arrange
            var isCalled = false;
            var tileMap = new TileMap(60, 50, "forest.png", 64, 64);
            var surface = new Mock<IDrawingSurface>();
            surface.Setup(s => s.AddTileMap(tileMap)).Callback(() => isCalled = true);

            var system = new DrawingSystem(surface.Object);

            // Act
            system.OnAddTileMap(tileMap);

            // Assert
            Assert.That(isCalled, Is.True);
        }

        [Test]
        public void OnRemoveTileMapRemovesItFromDrawingSurface()
        {
            // Arrange
            var isCalled = false;
            var tileMap = new TileMap(60, 50, "forest.png", 64, 64);
            var surface = new Mock<IDrawingSurface>();
            surface.Setup(s => s.RemoveTileMap(tileMap)).Callback(() => isCalled = true);
            var system = new DrawingSystem(surface.Object);
            system.OnAddTileMap(tileMap);

            // Act
            system.OnRemoveTileMap(tileMap);

            // Assert
            Assert.That(isCalled, Is.True);
        }
    }
}