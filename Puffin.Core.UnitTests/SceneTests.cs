using System;
using Moq;
using NUnit.Framework;
using Puffin.Core.Drawing;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Systems;
using Puffin.Core.Events;
using Puffin.Core.IO;
using Puffin.Core.Tiles;

namespace Puffin.Core.UnitTests
{
    [TestFixture]
    public class SceneTests
    {
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
        public void InitializeCallsReady()
        {
            // Arrange
            var scene = new Mock<Scene>() { CallBase = true };
            var onReadyCalled = false;
            scene.Setup(s => s.Ready()).Callback(() => onReadyCalled = true);
            var displaySystem = new Mock<DrawingSystem>();
            
            // Act
            scene.Object.Initialize(new ISystem[] { displaySystem.Object }, null, null);

            // Assert
            Assert.That(onReadyCalled, Is.True);
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
            scene.Initialize(
                new ISystem[] { drawingSystem.Object, audioSystem.Object },
                new Mock<IMouseProvider>().Object, new Mock<IKeyboardProvider>().Object);
            var elapsed = TimeSpan.FromSeconds(10);

            // Act
            scene.OnUpdate(elapsed);

            // Assert. Called several times (in chunks/increments of 0.15s)
            drawingSystem.Verify(d => d.OnUpdate(TimeSpan.FromMilliseconds(150)), Times.AtLeastOnce());
        }

        [Test]
        public void OnUpdateCallsSceneUpdate()
        {
            // Arrange
            bool calledUpdate = false;
            var scene = new Mock<Scene>() { CallBase = true };
            scene.Setup(s => s.Update(It.IsAny<float>())).Callback(() => calledUpdate = true);

            // Act
            scene.Object.OnUpdate(TimeSpan.FromSeconds(1));

            // Assert
            Assert.That(calledUpdate, Is.True);
        }

        [Test]
        public void OnDrawCallsDrawOnDrawingSystem()
        {
            // Arrange
            var elapsed = TimeSpan.FromSeconds(153);
            var drawingSystem = new Mock<DrawingSystem>();
            var scene = new Scene();
            scene.Initialize(new ISystem[] { drawingSystem.Object },
                new Mock<IMouseProvider>().Object, new Mock<IKeyboardProvider>().Object);

            // Act
            scene.OnDraw(elapsed, true);

            // Assert
            drawingSystem.Verify(d => d.OnDraw(elapsed, 0x000000, null, true), Times.Once());
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
            var called = false;
            var scene = new Scene();
            scene.Initialize(
                new ISystem[] { new DrawingSystem() },
                new Mock<IMouseProvider>().Object, new Mock<IKeyboardProvider>().Object);
            scene.OnMouseClick = (clickType) => called = true;

            // Act
            scene.EventBus.Broadcast(EventBusSignal.MouseClicked, ClickType.LeftClick);

            // Assert
            Assert.That(called, Is.True);
        }
        
        [Test]
        public void OnActionPressedFiresOnActionPressedEvent()
        {
            // Arrange
            var called = false;
            var actual = FakeAction.Reset;

            var scene = new Scene();
            scene.Initialize(
                new ISystem[] { new DrawingSystem() },
                new Mock<IMouseProvider>().Object, new Mock<IKeyboardProvider>().Object);

            scene.OnActionPressed = (e) =>
            {
                called = true;
                actual = (FakeAction)e;
            };

            // Act
            scene.EventBus.Broadcast(EventBusSignal.ActionPressed, FakeAction.Clear);
            
            // Assert
            Assert.That(called, Is.True);
            Assert.That(actual, Is.EqualTo(FakeAction.Clear));
        }

        [Test]
        public void OnActionReleasedFiresOnActionPressedEvent()
        {
            // Arrange
            var called = false;
            var actual = FakeAction.Reset;

            var scene = new Scene();
            scene.Initialize(
                new ISystem[] { new DrawingSystem() },
                new Mock<IMouseProvider>().Object, new Mock<IKeyboardProvider>().Object);
                
            scene.OnActionReleased = (e) =>
            {
                called = true;
                actual = (FakeAction)e;
            };

            // Act
            scene.EventBus.Broadcast(EventBusSignal.ActionReleased, FakeAction.Clear);
            
            // Assert
            Assert.That(called, Is.True);
            Assert.That(actual, Is.EqualTo(FakeAction.Clear));
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
            Assert.That(scene.IsActionDown(PuffinAction.Right), Is.False);
        }

        [Test]
        public void DrawCallsAddToFps()
        {
            // Arrange
            var drawingSystem = new Mock<DrawingSystem>().Object;
            var scene = new Scene();
            scene.Initialize(new ISystem[] { drawingSystem }, null, null);

            // Act
            scene.OnDraw(TimeSpan.Zero, true);
            scene.OnDraw(TimeSpan.Zero, true);
            scene.OnDraw(TimeSpan.Zero, true);

            // TODO: swap this out with an injected time provider
            System.Threading.Thread.Sleep(1000);
            scene.OnUpdate(TimeSpan.Zero);

            // Assert
            Assert.That(scene.Fps, Is.GreaterThan(0));
        }

        [Test]
        public void AddAddsTileMapToDrawingSystem()
        {
            // Arrange
            var isCalled = false;
            var system = new Mock<DrawingSystem>(new Mock<IDrawingSurface>().Object);
            var scene = new Scene();
            scene.Initialize(new ISystem[] { system.Object }, new Mock<IMouseProvider>().Object, new Mock<IKeyboardProvider>().Object);
            var tileMap = new TileMap(10, 5, "outdoors.png", 16, 16);

            system.Setup(s => s.OnAddTileMap(tileMap)).Callback(() => isCalled = true);

            // Act
            scene.Add(tileMap);

            // Assert
            Assert.That(isCalled, Is.True);
        }

        [Test]
        public void RemoveRemovesTileMapFromDrawingSystem()
        {
            // Arrange
            var isCalled = false;
            var system = new Mock<DrawingSystem>(new Mock<IDrawingSurface>().Object);
            var scene = new Scene();
            scene.Initialize(new ISystem[] { system.Object }, new Mock<IMouseProvider>().Object, new Mock<IKeyboardProvider>().Object);
            var tileMap = new TileMap(10, 5, "outdoors.png", 16, 16);

            system.Setup(s => s.OnRemoveTileMap(tileMap)).Callback(() => isCalled = true);
            scene.Add(tileMap);

            // Act
            scene.Remove(tileMap);

            // Assert
            Assert.That(isCalled, Is.True);
        }

        [Test]
        public void InitializeCallsOnAddTileMap()
        {
            // Arrange
            var tileMap = new TileMap(10, 5, "outdoors.png", 16, 16);

            var isCalled = false;
            var system = new Mock<DrawingSystem>(new Mock<IDrawingSurface>().Object);
            var scene = new Scene();
            scene.Add(tileMap);

            system.Setup(s => s.OnAddTileMap(tileMap)).Callback(() => isCalled = true);

            // Act
            scene.Initialize(new ISystem[] { system.Object }, new Mock<IMouseProvider>().Object, new Mock<IKeyboardProvider>().Object);

            // Assert
            Assert.That(isCalled, Is.True);
        }

        [Test]
        public void UpdateCallsEntityOnUpdateActions()
        {
            // Arrange
            float totalUpdatesSeconds = 0;
            var e = new Entity();
            e.OnUpdate((elapsed) => totalUpdatesSeconds += elapsed);
            var scene = new Scene();
            scene.Initialize(
                new ISystem[] { new DrawingSystem() },
                new Mock<IMouseProvider>().Object, new Mock<IKeyboardProvider>().Object);
            scene.Add(e);

            // Act
            scene.OnUpdate(TimeSpan.FromSeconds(1));
            
            // Assert
            Assert.That(totalUpdatesSeconds, Is.EqualTo(1));
        }

        [Test]
        public void ReadySetsUpKeyboardEventHandlers()
        {
            // Arrange
            var keyboardProvider = new Mock<IKeyboardProvider>();
            var drawingSystem = new Mock<DrawingSystem>().Object;
            var invoked = false;

            var scene = new Scene();
            scene.OnActionPressed = (data) => invoked = true;
            scene.Initialize(new ISystem[] { drawingSystem }, null, keyboardProvider.Object);
            scene.EventBus.Broadcast(PuffinAction.Down);

            // Shouldn't be called yet
            Assert.That(invoked, Is.False);
            
            // Act
            // Call ready, which sets up event handlers
            scene.Ready();
            scene.EventBus.Broadcast(EventBusSignal.ActionPressed);
            
            Assert.That(invoked, Is.True);
        }

        [Test]
        public void TweenPositionAddsTweenToTweenManager()
        {
            // Only way to test is to see who's updated
            var scene = new Scene();
            scene.Initialize(
                new ISystem[] { new DrawingSystem() },
                new Mock<IMouseProvider>().Object, new Mock<IKeyboardProvider>().Object);

            var e = new Entity();
            bool isCalled = false;
            scene.Add(e);

            // Act
            scene.TweenPosition(e, new System.Tuple<float, float>(50, 40), new System.Tuple<float, float>(45, 95), 1, () => isCalled = true);

            // Assert
            scene.Update(1);
            Assert.That(e.X, Is.EqualTo(45));
            Assert.That(e.Y, Is.EqualTo(95));
            Assert.That(isCalled, Is.True);
        }

        [Test]
        public void ShowSubSceneSetsSubScene()
        {
            var drawingSystem = new Mock<DrawingSystem>().Object;
            var scene = new Scene();
            scene.Initialize(new ISystem[] { drawingSystem }, null, new Mock<IKeyboardProvider>().Object);
            var subScene = new Scene();
            
            scene.ShowSubScene(subScene);
            
            Assert.That(scene.SubScene, Is.EqualTo(subScene));
        }

        [Test]
        public void ShowSubSceneUnsetsPreviousSubScene()
        {
            // Assert
            var drawingSystem = new Mock<DrawingSystem>().Object;
            var scene = new Scene();
            scene.Initialize(new ISystem[] { drawingSystem }, null, new Mock<IKeyboardProvider>().Object);
            var wrongSubScene = new Scene();
            var rightSubScene = new Scene();
            
            // Act
            scene.ShowSubScene(wrongSubScene);
            scene.ShowSubScene(rightSubScene);
            
            // Assert
            Assert.That(scene.SubScene, Is.EqualTo(rightSubScene));
        }

        [Test]
        public void HideSubSceneRemovesSubScene()
        {
            var drawingSystem = new Mock<DrawingSystem>().Object;
            var scene = new Scene();
            scene.Initialize(new ISystem[] { drawingSystem }, null, new Mock<IKeyboardProvider>().Object);
            var subScene = new Scene();
            scene.ShowSubScene(subScene);
            
            // Act
            scene.HideSubScene();
            
            // Assert
            Assert.That(scene.SubScene, Is.Null);
        }

        [Test]
        public void HideSubSceneBroadcastsHideEvent()
        {
            var isCalled = false;
            var drawingSystem = new Mock<DrawingSystem>().Object;
            var scene = new Scene();
            scene.Initialize(new ISystem[] { drawingSystem }, null, new Mock<IKeyboardProvider>().Object);
            var subScene = new Scene();
            scene.ShowSubScene(subScene);
            var bus = scene.EventBus;
            bus.Subscribe(EventBusSignal.SubSceneHidden, (data) => isCalled = true);
            
            // Act
            scene.HideSubScene();
            
            // Assert
            Assert.That(isCalled, Is.True);
        }

        [Test]
        public void ReadyCallsReadyOnEntities()
        {
            // Arrange
            bool isE1Ready = false;
            bool isE2Ready = false;
            var e1 = new Entity();
            e1.OnReady(() => isE1Ready = true);
            var e2 = new Entity();
            e2.OnReady(() => isE2Ready = true);

            var scene = new Scene();
            scene.Add(e1);
            scene.Add(e2);

            // Act
            scene.Ready();

            // Assert
            Assert.That(isE1Ready, Is.True);
            Assert.That(isE2Ready, Is.True);
        }

        [Test]
        public void AddEntityCallsReadyIfSceneIsReady()
        {
            // Arrange
            var scene = new Scene();
            scene.Initialize(
                new ISystem[] { new Mock<DrawingSystem>().Object },
                new Mock<IMouseProvider>().Object, new Mock<IKeyboardProvider>().Object);

            var isReadyCalled = false;
            var e1 = new Entity();
            e1.OnReady(() => isReadyCalled = true);

            // Act
            scene.Add(e1);

            // Assert.
            Assert.That(isReadyCalled, Is.True);
        }

        [Test]
        public void AddTileMapAddsAndSetsScene()
        {
            var scene = new Scene();
            var tileset = new TileMap(28, 10, "jungle.png", 64, 48);
            
            scene.Add(tileset);

            Assert.That(tileset.Scene, Is.EqualTo(scene));
        }

        enum FakeAction
        {
            Reset,
            Clear,
        }
    }
}