using System;
using NUnit.Framework;
using Puffin.UI.Controls;

namespace Puffin.UI.UnitTests.Controls
{
    [TestFixture]
    public class HorizontalProgressBarTests
    {
        [TestCase((string)null)]
        [TestCase("")]
        [TestCase("    ")]
        public void ConstructorThrowsIfImageFileNameIsBlank(string imageFileName)
        {
            Assert.Throws<ArgumentException>(() => new HorizontalProgressBar(true, imageFileName, 0xFF0000, 99, 16, 0, 0));
        }

        [TestCase(-99)]
        [TestCase(-1)]
        [TestCase(0)]
        public void ConstructorThrowsIfInnerWidthIsNonPositive(int innerWidth)
        {
            Assert.Throws<ArgumentException>(() => new HorizontalProgressBar(false, "flat-bar.png", 0xFFBB00, innerWidth, 16, 0, 0));
        }

        [TestCase(-99)]
        [TestCase(-1)]
        [TestCase(0)]
        public void ConstructorThrowsIfBarHeightIsNonPositive(int barHeight)
        {
            Assert.Throws<ArgumentException>(() => new HorizontalProgressBar(true, "flat-bar.png", 0xFFBB00, 100, barHeight, 0, 0));
        }

        [Test]
        public void ValueGetterAndSetterWork()
        {
            var bar = new HorizontalProgressBar(true, "plain.png", 0xFFFFFF, 32, 8, 4, 4);
            bar.Value = 18;
            Assert.That(bar.Value, Is.EqualTo(18));
        }

        [TestCase(-99)]
        [TestCase(-3)]
        [TestCase(0)]
        public void ValueSetterSetsNonPositiveValuesToZero(int value)
        {
            var bar = new HorizontalProgressBar(true, "plain.png", 0xFFFFFF, 32, 8, 4, 4);
            bar.Value = value;
            Assert.That(bar.Value, Is.Zero);
        }

        [TestCase(99)]
        [TestCase(13)]
        [TestCase(1)]
        public void ValueSetterSetsValuesGreaterThanMaxToMax(int value)
        {
            const int max = 99;
            var bar = new HorizontalProgressBar(true, "plain.png", 0xFFFFFF, max, 10, 4, 4);
            bar.Value = max + value;
            Assert.That(bar.Value, Is.EqualTo(max));
        }
    }
}