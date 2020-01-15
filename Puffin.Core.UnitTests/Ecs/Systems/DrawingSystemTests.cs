using Moq;
using NUnit.Framework;
using Puffin.Core.Drawing;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Core.Ecs.Systems;

namespace Puffin.Core.UnitTests
{
    [TestFixture]
    public class SpriteDrawingSystemTests
    {
        [Test]
        public void OnUpdateCallsDrawAll()
        {
            // Arrange
            var drawingSurface = new Mock<IDrawingSurface>();
            var system = new DrawingSystem(drawingSurface.Object);

            // Act
            system.OnUpdate();

            // Assert
            drawingSurface.Verify(d => d.DrawAll(), Times.Once());
        }
    }
}