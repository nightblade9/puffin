using NUnit.Framework;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;

namespace Puffin.Core.UnitTests.Ecs
{
    [TestFixture]
    public class EntityTests
    {
        [Test]
        public void GetIfHasReturnsSetComponents()
        {
            var expected = new StringComponent("hi");
            var e = new Entity();
            e.Set(expected);

            Assert.That(e.GetIfHas<StringComponent>(), Is.EqualTo(expected));
        }

        [Test]
        public void GetIfHasReturnsNullIfComponentIsntSet()
        {
            var e = new Entity();
            Assert.That(e.GetIfHas<StringComponent>(), Is.Null);
        }

        [Test]
        public void SetOverridesPreviousComponentOfThatType()
        {
            var expected = new StringComponent("pass!!");
            var e = new Entity();
            e.Set(new StringComponent("fail"));
            e.Set(expected);

            Assert.That(e.GetIfHas<StringComponent>(), Is.EqualTo(expected));
        }

        [Test]
        public void RemoveRemovesSetComponent()
        {
            var e = new Entity();
            e.Set(new StringComponent("here today, gone tomorrow"));
            e.Remove<StringComponent>();

            Assert.That(e.GetIfHas<StringComponent>(), Is.Null);
        }
    }

    // TODO: delete later
    class StringComponent : Component
    {
        public string Value { get; set; }
        public StringComponent(string value)
        {
            this.Value = value;
        }
    }
}