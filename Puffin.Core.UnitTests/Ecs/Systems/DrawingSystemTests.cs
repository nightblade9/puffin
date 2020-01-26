using System;
using Moq;
using NUnit.Framework;
using Puffin.Core.Drawing;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Systems;

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
            system.OnDraw(TimeSpan.Zero);

            // Assert
            drawingSurface.Verify(d => d.DrawAll(0x000000), Times.Once());
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
    }
}