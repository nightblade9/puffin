using System;
using NUnit.Framework;
using Puffin.Core.Ecs.Components;
using Puffin.UI.Controls;

namespace Puffin.UI.UnitTests.Controls
{
    [TestFixture]
    public class HorizontalSliderUnitTests
    {
        [TestCase((string)null)]
        [TestCase("")]
        [TestCase("     ")]
        public void ConstructorThrowsIfImageFileNameIsEmpty(string fileName)
        {
            Assert.Throws<ArgumentException>(() => new HorizontalSlider(true, fileName, 0x0, 32, 1, 10));
        }

        [TestCase(-100)]
        [TestCase(-1)]
        [TestCase(0)]
        public void ConstructorThrowsIfWidthIsNonPositive(int width)
        {
            Assert.Throws<ArgumentException>(() => new HorizontalSlider(true, "hande.png", 0x0, width, 1, 10));
        }

        [TestCase(-10)]
        [TestCase(0)]
        [TestCase(1)]
        public void ConstructorThrowsIfMaxIsLessThanOrEqualToMin(int max)
        {
            const int min = 1;
            Assert.Throws<ArgumentException>(() => new HorizontalSlider(true, "hande.png", 0x0, 32, min, max));
        }

        [Test]
        public void ConstructorAddsRequiredComponents()
        {
            var slider = new HorizontalSlider(true, "file.png", 0xFF0000, 100, 1, 7);
            Assert.That(slider.Get<SpriteComponent>(), Is.Not.Null);
            Assert.That(slider.Get<ColourComponent>(), Is.Not.Null);
            Assert.That(slider.Get<MouseComponent>(), Is.Not.Null);
        }

        [TestCase(-10)]
        [TestCase(12)]
        [TestCase(3)]
        public void SettingValueSetsValue(int expectedValue)
        {
            var slider = new HorizontalSlider(true, "file.png", 0xFF0000, 100, -20, 20);
            slider.Value = expectedValue;
            Assert.That(slider.Value, Is.EqualTo(expectedValue));
        }

        [TestCase(-10)]
        [TestCase(0)]
        [TestCase(3)]
        public void SettingValueBelowMinimumSetsValueToMinimum(int value)
        {
            const int min = 3;
            var slider = new HorizontalSlider(true, "file.png", 0xFF0000, 100, min, 7);
            slider.Value = value;
            Assert.That(slider.Value, Is.EqualTo(min));
        }

        [TestCase(-10)]
        [TestCase(0)]
        [TestCase(3)]
        public void SettingValueAboveMaximumSetsValueToMaximum(int value)
        {
            const int max = -20;
            var slider = new HorizontalSlider(true, "file.png", 0xFF0000, 100, -30, max);
            slider.Value = value;
            Assert.That(slider.Value, Is.EqualTo(max));
        }

        [Test]
        public void OnValueChangedCallsOnlyIfValueChanges()
        {
            // Arrange
            var expectedValue = 17;
            var actualValue = -99;
            var slider = new HorizontalSlider(true, "back.png", 0xFFFFCC, 100, 0, 20);
            slider.Value = 10;
            slider.OnValueChanged((newValue) => actualValue = newValue);

            // Act
            slider.Value = 10; // no change
            Assert.That(actualValue, Is.EqualTo(-99));

            slider.Value = expectedValue;
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }
    }
}