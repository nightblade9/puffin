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
        // TODO: move into drawing system
        [Test]
        public void OnUpdateDrawsSpritesAdded()
        {
            // Arrange
            var s1 = new SpriteComponent("galaxy.jpg");
            var s2 = new SpriteComponent("asteroid.png");
            var e1 = new Entity().Set(s1);
            var e2 = new Entity().Set(s2);
            var e3 = new Entity(); // No sprite

            var drawingSurface = new Mock<IDrawingSurface>();
            var system = new SpriteDrawingSystem(drawingSurface.Object);
            system.OnAddEntity(e1);
            system.OnAddEntity(e2);
            system.OnAddEntity(e3);

            // Act
            system.OnUpdate();

            // Assert
            drawingSurface.Verify(d => d.Draw(s1));
            drawingSurface.Verify(d => d.Draw(s2));
            drawingSurface.Verify(d => d.Draw(It.IsAny<SpriteComponent>()), Times.Exactly(2));
        }
    }
}