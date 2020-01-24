using System;
using Moq;
using NUnit.Framework;
using Puffin.Core.Ecs.Systems;
using Puffin.Core.IO;

namespace Puffin.Core.UnitTests.Ecs.Systems
{
    [TestFixture]
    public class AudioSystemTests
    {
        [Test]
        public void OnUpdateUpdatesAudioPlayer()
        {
            // Arrange
            var audioPlayer = new Mock<IAudioPlayer>();

            var system = new AudioSystem(audioPlayer.Object);

            // Act
            system.OnUpdate(TimeSpan.Zero);

            //
            audioPlayer.Verify(a => a.OnUpdate(), Times.Once());
        }
    }
}