using Moq;
using NUnit.Framework;
using Puffin.Core;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Core.Ecs.Systems;
using Puffin.Core.Events;
using Puffin.Core.IO;
using Puffin.UI.Controls;

namespace Puffin.UI.UnitTests.Controls
{
    [TestFixture]
    public class ButtonTests
    {
        [Test]
        public void ButtonConstructorSetsSpriteTextAndMouseComponents()
        {
            // Arrange
            var eventBus = new EventBus();
            var mouseProvider = new Mock<IMouseProvider>();
            // Returns coordinates within the sprite (currently 128x48)
            mouseProvider.Setup(m => m.MouseCoordinates).Returns(new System.Tuple<int, int>(17, 32));
            mouseProvider.Setup(m => m.UiMouseCoordinates).Returns(new System.Tuple<int, int>(19, 30));

            var clicked = false;
            var button = new Button("click me!", 0, 0, () => clicked = true);
            var mouseSystem = new MouseSystem(eventBus, mouseProvider.Object);
            mouseSystem.OnAddEntity(button);

            // Act
            eventBus.Broadcast(EventBusSignal.MouseClicked, null);

            // Assert
            Assert.That(clicked, Is.True);
            Assert.That(button.Get<SpriteComponent>(), Is.Not.Null);
            Assert.That(button.Get<TextLabelComponent>(), Is.Not.Null);
            Assert.That(button.Get<MouseComponent>(), Is.Not.Null);

            var label = button.Get<TextLabelComponent>();
            Assert.That(label.Text, Is.EqualTo("click me!"));

            var sprite = button.Get<SpriteComponent>();
            Assert.That(sprite.FileName.EndsWith("Button.png"));
        }
    }
}
