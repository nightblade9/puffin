using System;
using NUnit.Framework;
using Puffin.Core.Ecs;
using Puffin.Core.Tweening;

namespace Puffin.Core.UnitTests.Tweening
{
    [TestFixture]
    public class TweenManagerTests
    {
        [Test]
        public void TweenPositionReplacesAndStopsTweenForThatEntity()
        {
            // Only way to test is to see who's updated
            var manager = new TweenManager();
            var e = new Entity();
            bool isCalled = false;

            manager.Tween(e, 0, new System.Tuple<float, float>(0, 0), new System.Tuple<float, float>(100, 100));
            
            // Act
            manager.Tween(e, 1, new System.Tuple<float, float>(50, 40), new System.Tuple<float, float>(45, 95), onTweenComplete: () => isCalled = true);

            // Assert
            // We can't directly check if the first tween is stopped; all we can do is observe the current position of the entity.
            manager.Update(1);
            Assert.That(e.X, Is.EqualTo(45));
            Assert.That(e.Y, Is.EqualTo(95));
            Assert.That(isCalled, Is.True);
        }

        [Test]
        public void UpdateUpdatesTween()
        {
            var manager = new TweenManager();
            var e = new Entity();
            manager.Tween(e, 10, new System.Tuple<float, float>(0, 0), new System.Tuple<float, float>(100, 200));

            // Act
            manager.Update(1);

            // Assert: takes 10s so we should have moved 10%
            Assert.That(e.X, Is.EqualTo(10));
            Assert.That(e.Y, Is.EqualTo(20));
        }

        [Test]
        public void UpdateRemovesCompletedTweens()
        {
            // Test that it's not updated after removal
            var manager = new TweenManager();
            var e = new Entity();
            manager.Tween(e, 10, new System.Tuple<float, float>(0, 0), new System.Tuple<float, float>(100, 200));

            // Act
            manager.Update(10);

            // Assert: takes 10s so we should have moved 10%
            Assert.That(e.X, Is.EqualTo(100));
            Assert.That(e.Y, Is.EqualTo(200));

            manager.Update(999);
            Assert.That(e.X, Is.EqualTo(100));
            Assert.That(e.Y, Is.EqualTo(200));
        }
    }
}