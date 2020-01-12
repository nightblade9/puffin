using Moq;
using NUnit.Framework;
using Puffin.Core.Drawing;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Systems;

namespace Puffin.Core.UnitTests
{
    [TestFixture]
    public class SceneTests
    {
        [Test]
        public void AddCallsOnAddEntityOnSystems()
        {
            // Arrange
            var drawingSystem = new Mock<ISystem>();
            var audioSystem = new Mock<ISystem>();

            var e1 = new Entity();
            var e2 = new Entity();
            
            var scene = new Scene(drawingSystem.Object, audioSystem.Object);

            // Act
            scene.Add(e1);
            scene.Add(e2);

            // Assert
            drawingSystem.Verify(d => d.OnAddEntity(e1), Times.Once());
            drawingSystem.Verify(d => d.OnAddEntity(e2), Times.Once());
        }

        public void OnUpdateCallsOnUpdateOnSystems()
        {
            // Arrange
            var drawingSystem = new Mock<ISystem>();
            var audioSystem = new Mock<ISystem>();
            
            var scene = new Scene(drawingSystem.Object, audioSystem.Object);

            // Act
            scene.OnUpdate(new Mock<IDrawingSurface>().Object);

            // Assert
            drawingSystem.Verify(d => d.OnUpdate(), Times.Once());
            drawingSystem.Verify(d => d.OnUpdate(), Times.Once());
        }
    }
}