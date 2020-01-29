using System;
using NUnit.Framework;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;

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
    }
}