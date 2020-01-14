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

        [Test]
        public void MoveSetsEntityCoordinates()
        {
            var e = new Entity();
            e.Move(1, 2);
            e.Move(200, 140);

            Assert.That(e.X, Is.EqualTo(200));
            Assert.That(e.Y, Is.EqualTo(140));
        }

        [Test]
        public void SettingXOrYTriggersSetCallbacks()
        {
            var timesCalledBack = 0;
            var callbackX = 0;
            var callbackY = 0;

            var e = new Entity();
            e.AddPositionChangeCallback((x, y) => {
                callbackX = x;
                callbackY = y;
                timesCalledBack++;
            });

            // Act/Assert
            e.X = 100;
            Assert.That(timesCalledBack, Is.EqualTo(1));
            Assert.That(callbackX, Is.EqualTo(100));

            e.Y = 293;
            Assert.That(timesCalledBack, Is.EqualTo(2));
            Assert.That(callbackY, Is.EqualTo(293));
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