using System;
using NUnit.Framework;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Core.Events;

namespace Puffin.Core.UnitTests.Ecs.Components
{
    [TestFixture]
    public class AudioComponentTests
    {
        [TestCase(-0.0001f)]
        [TestCase(-3f)]
        [TestCase(-3192)]
        [TestCase(1.01f)]
        [TestCase(1.78f)]
        [TestCase(5)]
        [TestCase(37)]
        public void PlayThrowsIfVolumeIsInvalid(float volume)
        {
            var e = new Entity();
            var audio = new AudioComponent(e, "bloop.wav");
            
            var ex = Assert.Throws<ArgumentException>(() => audio.Play(volume));
        }

        [TestCase(-1.00001f)]
        [TestCase(-12f)]
        [TestCase(1.0001f)]
        [TestCase(10f)]
        public void PlayThrowsIfPitchIsInvalid(float pitch)
        {
            var e = new Entity();
            var audio = new AudioComponent(e, "bloop.wav");
            
            var ex = Assert.Throws<ArgumentException>(() => audio.Play(1, pitch));
        }

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
                Assert.That(actual.Volume, Is.EqualTo(0.74f));
                Assert.That(actual.Pitch, Is.EqualTo(0.3f));
                Assert.That(actual, Is.EqualTo(audio));
            });

            // Act
            audio.Play(0.74f, 0.3f);

            Assert.That(isCalled, Is.True);
        }
    }
}