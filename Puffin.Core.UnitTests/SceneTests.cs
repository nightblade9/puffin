using Moq;
using NUnit.Framework;
using Puffin.Core.Drawing;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;

namespace Puffin.Core.UnitTests
{
    [TestFixture]
    public class SceneTests
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

            var scene = new Scene();
            scene.Add(e1);
            scene.Add(e2);
            scene.Add(e3);

            var drawingSurface = new Mock<IDrawingSurface>();

            // Act
            scene.OnUpdate(drawingSurface.Object);

            // Assert
            drawingSurface.Verify(d => d.Draw(s1));
            drawingSurface.Verify(d => d.Draw(s2));
            drawingSurface.Verify(d => d.Draw(It.IsAny<SpriteComponent>()), Times.Exactly(2));
        }
    }
}