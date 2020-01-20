using Moq;
using NUnit.Framework;
using Puffin.Core.Ecs;
using Puffin.Core.IO;

namespace Puffin.Core.UnitTests.Ecs
{
    [TestFixture]
    public class KeyboardComponentTests
    {
        [TearDown]
        public void ResetDependencyInjectionBindings()
        {
            DependencyInjection.Reset();
        }

        [Test]
        public void IsKeyDownReturnsTrueIfKeysAreDownForAction()
        {
            // Depends on default mapping for PuffinGame.
            var provider = new Mock<IKeyboardProvider>();
            DependencyInjection.Kernel.Bind<IKeyboardProvider>().ToConstant(provider.Object);

            provider.Setup(p => p.IsActionDown(PuffinAction.Up)).Returns(true);
            provider.Setup(p => p.IsActionDown(PuffinAction.Left)).Returns(true);
            provider.Setup(p => p.IsActionDown(PuffinAction.Down)).Returns(false);
            provider.Setup(p => p.IsActionDown(PuffinAction.Right)).Returns(false);

            var component = new KeyboardComponent(new Entity());

            // Act/Assert
            Assert.That(component.IsActionDown(PuffinAction.Up), Is.True);
            Assert.That(component.IsActionDown(PuffinAction.Left), Is.True);
            Assert.That(component.IsActionDown(PuffinAction.Down), Is.False);
            Assert.That(component.IsActionDown(PuffinAction.Right), Is.False);
        }

        [Test]
        public void IsKeyDownReturnsTrueForCustomEnums()
        {
            var provider = new Mock<IKeyboardProvider>();
            DependencyInjection.Kernel.Bind<IKeyboardProvider>().ToConstant(provider.Object);

            provider.Setup(p => p.IsActionDown(CustomAction.Proceed)).Returns(true);
            provider.Setup(p => p.IsActionDown(CustomAction.Cancel)).Returns(false);

            var component = new KeyboardComponent(new Entity());

            // Act/Assert
            Assert.That(component.IsActionDown(CustomAction.Proceed), Is.True);
            Assert.That(component.IsActionDown(CustomAction.Cancel), Is.False);
        }

        enum CustomAction
        {
            Proceed,
            Cancel,
        }
    }
}