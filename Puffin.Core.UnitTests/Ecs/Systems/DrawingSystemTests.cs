using System;
using Moq;
using NUnit.Framework;
using Puffin.Core.Drawing;
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
            drawingSurface.Verify(d => d.DrawAll(), Times.Once());
        }
    }
}