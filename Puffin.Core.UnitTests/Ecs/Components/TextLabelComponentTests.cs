using System;
using NUnit.Framework;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Core.Events;

namespace Puffin.Core.UnitTests.Ecs
{
    [TestFixture]
    public class TextLabelComponentTests
    {
        [TestCase(-11)]
        [TestCase(0)]
        public void SetFontSizeThrowsIfFontSizeIsNonPositive(int value)
        {
            Assert.Throws<ArgumentException>(() => new TextLabelComponent(new Entity(), "hi!").FontSize = value);
        }

        [Test]
        public void UpdatingTextBroadcastsEventForRecalculatingWidth()
        {
            // Arrange
            var t = new Entity().Label("Hi!");
            var s = new Scene();
            var isBroadcast = false;
            s.EventBus.Subscribe(EventBusSignal.LabelTextChanged, (e) => isBroadcast = true);
            s.Add(t);
            
            // Act
            t.Get<TextLabelComponent>().Text = "Hee!";

            // Assert
            Assert.That(isBroadcast);
        }

        [Test]
        public void UpdatingFontNameBroadcastsEvent()
        {
            // Arrange
            var t = new Entity().Label("Hi!");
            var s = new Scene();
            var isBroadcast = false;
            s.EventBus.Subscribe(EventBusSignal.LabelFontChanged, (e) => isBroadcast = true);
            s.Add(t);
            
            // Act
            t.Get<TextLabelComponent>().FontName = "SuperFont";

            // Assert
            Assert.That(isBroadcast);
        }

        [Test]
        public void UpdatingFontSizeBroadcastsEvent()
        {
            // Arrange
            var t = new Entity().Label("Hi!");
            var s = new Scene();
            var isBroadcast = false;
            s.EventBus.Subscribe(EventBusSignal.LabelFontChanged, (e) => isBroadcast = true);
            s.Add(t);
            
            // Act
            t.Get<TextLabelComponent>().FontSize = 22;

            // Assert
            Assert.That(isBroadcast);
        }
    }
}