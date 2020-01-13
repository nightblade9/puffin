using Moq;
using NUnit.Framework;
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
            
            var scene = new Scene();
            scene.Initialize(drawingSystem.Object, audioSystem.Object);

            // Act
            scene.Add(e1);
            scene.Add(e2);

            // Assert
            drawingSystem.Verify(d => d.OnAddEntity(e1), Times.Once());
            drawingSystem.Verify(d => d.OnAddEntity(e2), Times.Once());
        }

        [Test]
        public void InitializeCallsOnAddEntityOnSystems()
        {
            // Arrange
            var drawingSystem = new Mock<ISystem>();
            var audioSystem = new Mock<ISystem>();

            var e1 = new Entity();
            var e2 = new Entity();
            
            var scene = new Scene();
            scene.Add(e1);
            scene.Add(e2);

            // Act
            scene.Initialize(drawingSystem.Object, audioSystem.Object);
            
            // Assert
            drawingSystem.Verify(d => d.OnAddEntity(e1), Times.Once());
            drawingSystem.Verify(d => d.OnAddEntity(e2), Times.Once());
        }

        public void OnUpdateCallsOnUpdateOnSystems()
        {
            // Arrange
            var drawingSystem = new Mock<ISystem>();
            var audioSystem = new Mock<ISystem>();
            
            var scene = new Scene();
            scene.Initialize(drawingSystem.Object, audioSystem.Object);

            // Act
            scene.OnUpdate();

            // Assert
            drawingSystem.Verify(d => d.OnUpdate(), Times.Once());
            drawingSystem.Verify(d => d.OnUpdate(), Times.Once());
        }
    }
}