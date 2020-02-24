using System;
using NUnit.Framework;
using Puffin.Core.Ecs;

namespace Puffin.Core.UnitTests.Tweening
{
    [TestFixture]
    public class TweenTests
    {
        [Test]
        public void ConstructorStartsTween()
        {
            var e = new Entity();
            var tween = new Puffin.Core.Tweening.Tween(e, new Tuple<float, float>(50, 60), new Tuple<float, float>(60, 70), 1);
            Assert.That(tween.IsRunning, Is.True);
        }

        [Test]
        public void StopAndStartStopAndStartTween()
        {
            var e = new Entity();
            var tween = new Puffin.Core.Tweening.Tween(e, new Tuple<float, float>(50, 60), new Tuple<float, float>(60, 70), 1);
            // Started

            tween.Stop();
            Assert.That(tween.IsRunning, Is.False);

            tween.Start();
            Assert.That(tween.IsRunning, Is.True);
        }

        [Test]
        public void StartSetsEntityCoordinatesToStartPosition()
        {
            var e = new Entity().Move(999, 998);
            var tween = new Puffin.Core.Tweening.Tween(e, new Tuple<float, float>(50, 60), new Tuple<float, float>(60, 70), 1);
            tween.Stop();

            e.Move(-999, -999);

            // Act
            tween.Start();

            // Assert
            Assert.That(e.X, Is.EqualTo(50));
            Assert.That(e.Y, Is.EqualTo(60));
        }

        [Test]
        public void StopInvokesStopCallbackAndDoesntChangeEntityCoordinates()
        {
            var e = new Entity().Move(999, 998);
            var isCalled = true;
            var tween = new Puffin.Core.Tweening.Tween(e, new Tuple<float, float>(50, 60), new Tuple<float, float>(60, 70), 1, () => isCalled = true);

            // Act
            tween.Stop();

            // Assert: stop didn't jump to the end coordinates
            Assert.That(e.X, Is.EqualTo(50));
            Assert.That(e.Y, Is.EqualTo(60));
            Assert.That(isCalled, Is.True);
        }

        [Test]
        public void UpdateMovesEntityProportionalToDurationAndElapsed()
        {
            const int startX = 500;
            const int startY = 600;
            const int stopX = 1000;
            const int stopY = 1000;
            const int duration = 1;
            const float elapsed = 0.25f;
            const float elapsedPercent = elapsed / duration;
            
            const int expectedX = startX + (int)((stopX - startX) * elapsedPercent);
            const int expectedY = startY + (int)((stopY - startY) * elapsedPercent);

            var e = new Entity();
            var tween = new Puffin.Core.Tweening.Tween(e, new Tuple<float, float>(startX, startY), new Tuple<float, float>(stopX, stopY), duration);

            // Act
            tween.Update(elapsed);

            // Assert
            Assert.That(e.X, Is.EqualTo(expectedX));
            Assert.That(e.Y, Is.EqualTo(expectedY));
        }

        [Test]
        public void UpdateDoesntOvershootButMovesEntityToEndPosition()
        {
            var e = new Entity();
            var tween = new Puffin.Core.Tweening.Tween(e, new Tuple<float, float>(0, 0), new Tuple<float, float>(-50, -40), 1);

            // Act
            tween.Update(2);

            // Assert
            Assert.That(e.X, Is.EqualTo(-50));
            Assert.That(e.Y, Is.EqualTo(-40));
        }
    }
}