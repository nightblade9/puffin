﻿using Moq;
using NUnit.Framework;
using Puffin.Core;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Core.IO;
using Puffin.UI.Controls;

namespace Puffin.UI.UnitTests.Controls
{
    [TestFixture]
    public class ButtonTests
    {
        [TearDown]
        public void ResetDependencyInjection()
        {
            DependencyInjection.Reset();
        }

        [Test]
        public void ButtonConstructorSetsSpriteTextAndMouseComponents()
        {
            // Arrange
            var eventBus = new EventBus();
            var mouseProvider = new Mock<IMouseProvider>();
            // Returns coordinates within the sprite (currently 128x48)
            mouseProvider.Setup(m => m.MouseCoordinates).Returns(new System.Tuple<int, int>(17, 32));
            DependencyInjection.Kernel.Bind<IMouseProvider>().ToConstant(mouseProvider.Object);

            var clicked = false;
            var button = new Button("click me!", () => clicked = true);

            // Act
            eventBus.Broadcast(EventBusSignal.MouseClicked, null);

            // Assert
            Assert.That(clicked, Is.True);
            Assert.That(button.GetIfHas<SpriteComponent>(), Is.Not.Null);
            Assert.That(button.GetIfHas<TextLabelComponent>(), Is.Not.Null);
            Assert.That(button.GetIfHas<MouseComponent>(), Is.Not.Null);

            var label = button.GetIfHas<TextLabelComponent>();
            Assert.That(label.Text, Is.EqualTo("click me!"));

            var sprite = button.GetIfHas<SpriteComponent>();
            Assert.That(sprite.FileName.EndsWith("Button.png"));
        }
    }
}