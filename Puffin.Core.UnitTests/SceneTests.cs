using Moq;
using NUnit.Framework;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Systems;
using Puffin.Core.IO;

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
            scene.Initialize(new ISystem[] { drawingSystem.Object, audioSystem.Object }, null);

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
            scene.Initialize(new ISystem[] { drawingSystem.Object, audioSystem.Object }, null);
            
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
            scene.Initialize(new ISystem[] { drawingSystem.Object, audioSystem.Object }, null);

            // Act
            scene.OnUpdate();

            // Assert
            drawingSystem.Verify(d => d.OnUpdate(), Times.Once());
            drawingSystem.Verify(d => d.OnUpdate(), Times.Once());
        }

        [Test]
        public void MouseCoordinatesReturnsCoordinatesFromMouseProvider()
        {
            var expectedCoordinates = new System.Tuple<int, int>(123, 21);
            var mouseProvider = new Mock<IMouseProvider>();
            mouseProvider.Setup(m => m.MouseCoordinates).Returns(expectedCoordinates);

            var scene = new Scene();
            scene.Initialize(new ISystem[0], mouseProvider.Object);

            Assert.That(scene.MouseCoordinates, Is.EqualTo(expectedCoordinates));
        }
    }
}