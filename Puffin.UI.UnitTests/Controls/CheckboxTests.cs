using System;
using Moq;
using NUnit.Framework;
using Puffin.Core.Ecs.Components;
using Puffin.Core.Ecs.Systems;
using Puffin.Core.Events;
using Puffin.Core.IO;
using Puffin.UI.Controls;

namespace Puffin.UI.UnitTests.Controls
{
    [TestFixture]
    public class CheckboxTests
    {
        [Test]
        public void ConstructorSetsSpriteWithCheckedAndMouseAndLabelComponents()
        {
            var c = new Checkbox(true, "unchecked.png", "checked.png", 32, 32, "Nerf Minions");

            Assert.That(c.Get<SpriteComponent>(), Is.Not.Null);
            Assert.That(c.Get<SpriteComponent>().FileName, Is.EqualTo("checked.png"));
            Assert.That(c.IsChecked, Is.True);
            Assert.That(c.Get<MouseComponent>(), Is.Not.Null);
            Assert.That(c.Get<TextLabelComponent>(), Is.Not.Null);
            Assert.That(c.Get<TextLabelComponent>().Text, Is.EqualTo("Nerf Minions"));
        }

        [Test]
        public void TogglingCheckedChangesSprite()
        {
            var c = new Checkbox(false, "unchecked.png", "checked.png", 32, 32, "");
            
            c.IsChecked = false;
            Assert.That(c.Get<SpriteComponent>().FileName, Is.EqualTo("unchecked.png"));

            c.IsChecked = true;
            Assert.That(c.Get<SpriteComponent>().FileName, Is.EqualTo("checked.png"));
        }
    }
}
