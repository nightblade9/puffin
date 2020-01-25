using System;
using Moq;
using NUnit.Framework;
using Puffin.Core.Drawing;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Systems;
using Puffin.Core.IO;

namespace Puffin.Core.UnitTests
{
    [TestFixture]
    public class SceneTests
    {
        [TearDown]
        public void ResetDependencyInjection()
        {
            DependencyInjection.Reset();
        }

        [Test]
        public void InitializeThrowsIfDrawingSystemIsMissing()
        {
            var audioSystem = new Mock<ISystem>();
            var scene = new Scene();
            
            Assert.Throws<InvalidOperationException>(() => scene.Initialize(
                new ISystem[] { audioSystem.Object },
                new Mock<IMouseProvider>().Object, new Mock<IKeyboardProvider>().Object));
        }

        [Test]
        public void AddCallsOnAddEntityOnSystems()
        {
            // Arrange
            var drawingSystem = new Mock<DrawingSystem>();
            var audioSystem = new Mock<ISystem>();

            var e1 = new Entity();
            var e2 = new Entity();
            
            var scene = new Scene();
            scene.Initialize(new ISystem[] { drawingSystem.Object, audioSystem.Object }, null, null);

            // Act
            scene.Add(e1);
            scene.Add(e2);

            // Assert
            drawingSystem.Verify(d => d.OnAddEntity(e1), Times.Once());
            drawingSystem.Verify(d => d.OnAddEntity(e2), Times.Once());
        }

        [Test]
        public void RemoveCallsRemoveOnSystems()
        {
            var entity = new Entity();
            var system = new Mock<ISystem>();
            var drawingSurface = new Mock<IDrawingSurface>();
            var drawingSystem = new DrawingSystem(drawingSurface.Object);

            var scene = new Scene();
            scene.Initialize(new ISystem[] { drawingSystem, system.Object }, null, null);
            scene.Add(entity);

            // Act
            scene.Remove(entity);

            // Assert
            system.Verify(s => s.OnRemoveEntity(entity), Times.Once());
            drawingSurface.Verify(s => s.RemoveEntity(entity), Times.Once());
        }

        [Test]
        public void InitializeCallsOnAddEntityOnSystems()
        {
            // Arrange
            var drawingSystem = new Mock<DrawingSystem>();
            var audioSystem = new Mock<ISystem>();

            var e1 = new Entity();
            var e2 = new Entity();
            
            var scene = new Scene();
            scene.Add(e1);
            scene.Add(e2);

            // Act
            scene.Initialize(new ISystem[] { drawingSystem.Object, audioSystem.Object }, null, null);
            
            // Assert
            drawingSystem.Verify(d => d.OnAddEntity(e1), Times.Once());
            drawingSystem.Verify(d => d.OnAddEntity(e2), Times.Once());
        }

        [Test]
        public void OnUpdateCallsOnUpdateOnSystems()
        {
            // Arrange
            var drawingSystem = new Mock<DrawingSystem>();
            var audioSystem = new Mock<ISystem>();
            
            var scene = new Scene();
            scene.Initialize(new ISystem[] { drawingSystem.Object, audioSystem.Object }, null, null);

            // Act
            scene.OnUpdate(TimeSpan.Zero);

            // Assert
            drawingSystem.Verify(d => d.OnUpdate(TimeSpan.Zero), Times.Once());
            drawingSystem.Verify(d => d.OnUpdate(TimeSpan.Zero), Times.Once());
        }

        [Test]
        public void OnUpdateCallsSceneUpdate()
        {
            // Arrange
            bool calledUpdate = false;
            var scene = new Mock<Scene>() { CallBase = true };
            scene.Setup(s => s.Update()).Callback(() => calledUpdate = true);

            // Act
            scene.Object.OnUpdate(TimeSpan.Zero);

            // Assert
            Assert.That(calledUpdate, Is.True);
        }

        [Test]
        public void OnDrawCallsDrawOnDrawingSystem()
        {
            // Arrange
            var elapsed = TimeSpan.FromMilliseconds(153);
            var drawingSystem = new Mock<DrawingSystem>();
            var scene = new Scene();
            scene.Initialize(new ISystem[] { drawingSystem.Object },
                new Mock<IMouseProvider>().Object, new Mock<IKeyboardProvider>().Object);

            // Act
            scene.OnDraw(elapsed);

            // Assert
            drawingSystem.Verify(d => d.OnDraw(elapsed), Times.Once());
        }

        [Test]
        public void MouseCoordinatesReturnsCoordinatesFromMouseProvider()
        {
            var expectedCoordinates = new System.Tuple<int, int>(123, 21);
            var mouseProvider = new Mock<IMouseProvider>();
            mouseProvider.Setup(m => m.MouseCoordinates).Returns(expectedCoordinates);
            var bus = new EventBus();
            var drawingSystem = new Mock<DrawingSystem>();

            var scene = new Scene();
            scene.Initialize(new ISystem[] { drawingSystem.Object }, mouseProvider.Object, null);

            Assert.That(scene.MouseCoordinates, Is.EqualTo(expectedCoordinates));
        }

        [Test]
        public void OnMouseClickFiresOnMouseClickEvent()
        {
            // Arrange
            var eventBus = new EventBus();
            var called = false;
            var scene = new Scene();
            scene.OnMouseClick = () => called = true;

            // Act
            eventBus.Broadcast(EventBusSignal.MouseClicked);

            // Assert
            Assert.That(called, Is.True);
        }

        [Test]
        public void IsActionDownReturnsValueFromKeyboardProvider()
        {
            // Arrange
            var keyboardProvider = new Mock<IKeyboardProvider>();
            keyboardProvider.Setup(k => k.IsActionDown(PuffinAction.Left)).Returns(true);
            var drawingSystem = new Mock<DrawingSystem>().Object;

            var scene = new Scene();
            scene.Initialize(new ISystem[] { drawingSystem }, null, keyboardProvider.Object);

            // Assert
            Assert.That(scene.IsActionDown(PuffinAction.Left), Is.True);
        }

        [Test]
        public void DrawCallsAddToFps()
        {
            // Arrange
            var drawingSystem = new Mock<DrawingSystem>().Object;
            var scene = new Scene();
            scene.Initialize(new ISystem[] { drawingSystem }, null, null);

            // Act
            scene.OnDraw(TimeSpan.Zero);
            scene.OnDraw(TimeSpan.Zero);
            scene.OnDraw(TimeSpan.Zero);

            // TODO: swap this out with an injected time provider
            System.Threading.Thread.Sleep(1000);
            scene.OnUpdate(TimeSpan.Zero);

            // Assert
            Assert.That(scene.Fps, Is.GreaterThan(0));
        }
    }
}