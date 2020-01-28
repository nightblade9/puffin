using NUnit.Framework;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;

namespace Puffin.Core.UnitTests.Ecs
{
    [TestFixture]
    public class EntityTests
    {
        [Test]
        public void GetReturnsSetComponents()
        {
            var expected = new StringComponent("hi");
            var e = new Entity();
            e.Set(expected);

            Assert.That(e.Get<StringComponent>(), Is.EqualTo(expected));
        }

        [Test]
        public void GetReturnsNullIfComponentIsntSet()
        {
            var e = new Entity();
            Assert.That(e.Get<StringComponent>(), Is.Null);
        }

        [Test]
        public void SetOverridesPreviousComponentOfThatType()
        {
            var expected = new StringComponent("pass!!");
            var e = new Entity();
            e.Set(new StringComponent("fail"));
            e.Set(expected);

            Assert.That(e.Get<StringComponent>(), Is.EqualTo(expected));
        }

        [Test]
        public void RemoveRemovesSetComponent()
        {
            var e = new Entity();
            e.Set(new StringComponent("here today, gone tomorrow"));
            e.Remove<StringComponent>();

            Assert.That(e.Get<StringComponent>(), Is.Null);
        }

        [Test]
        public void MoveSetsEntityCoordinates()
        {
            var e = new Entity();
            e.Move(1, 2);
            e.Move(200, 140);

            Assert.That(e.X, Is.EqualTo(200));
            Assert.That(e.Y, Is.EqualTo(140));
        }
    }

    // TODO: delete later
    class StringComponent : Component
    {
        public string Value { get; set; }
        public StringComponent(string value)
        : base(new Entity())
        {
            this.Value = value;
        }
    }
}