using NUnit.Framework;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Core.Events;

namespace Puffin.Core.UnitTests.Ecs.Components
{
    [TestFixture]
    public class AudioComponentTests
    {
        [Test]
        public void PlayTriggersEventBusAndSetsPitch()
        {
            // Arrange
            var scene = new Scene();
            var isCalled = false;
            var e = new Entity();
            var audio = new AudioComponent(e, "buzz.wav");
            scene.Add(e);

            scene.EventBus.Subscribe(EventBusSignal.PlayAudio, (data) => {
                isCalled = true;
                var actual = data as AudioComponent;
                Assert.That(actual.Pitch, Is.EqualTo(0.74f));
                Assert.That(actual, Is.EqualTo(audio));
            });

            // Act
            audio.Play(0.74f);

            Assert.That(isCalled, Is.True);
        }
    }
}