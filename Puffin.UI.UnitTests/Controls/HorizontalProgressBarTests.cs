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
            Assert.Throws<ArgumentException>(() => new HorizontalProgressBar(imageFileName, 0xFF0000, 99, 0, 0));
        }

        [TestCase(-99)]
        [TestCase(-1)]
        [TestCase(0)]
        public void ConstructorThrowsIfInnerWidthIsNonPositive(int innerWidth)
        {
            Assert.Throws<ArgumentException>(() => new HorizontalProgressBar("flat-bar.png", 0xFFBB00, innerWidth, 0, 0));
        }

        [Test]
        public void ValueGetterAndSetterWork()
        {
            var bar = new HorizontalProgressBar("plain.png", 0xFFFFFF, 32, 4, 4);
            bar.Value = 18;
            Assert.That(bar.Value, Is.EqualTo(18));
        }

        [TestCase(-99)]
        [TestCase(-3)]
        [TestCase(0)]
        public void ValueSetterSetsNonPositiveValuesToZero(int value)
        {
            var bar = new HorizontalProgressBar("plain.png", 0xFFFFFF, 32, 4, 4);
            bar.Value = value;
            Assert.That(bar.Value, Is.Zero);
        }

        [TestCase(99)]
        [TestCase(13)]
        [TestCase(1)]
        public void ValueSetterSetsValuesGreaterThanMaxToMax(int value)
        {
            const int max = 99;
            var bar = new HorizontalProgressBar("plain.png", 0xFFFFFF, max, 4, 4);
            bar.Value = max + value;
            Assert.That(bar.Value, Is.EqualTo(max));
        }
    }
}