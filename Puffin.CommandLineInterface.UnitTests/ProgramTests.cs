using System;
using System.IO;
using NUnit.Framework;

namespace Puffin.CommandLineInterface.UnitTests
{
    [TestFixture]
    public class ProgramTests
    {
        // Private value copy/pasted from Program.cs
        private const string DEFAULT_NEW_PROJECT_NAME = "NewPuffin";

        [Test]
        public void ProgramCreatesDirectoryUsingDefaultNameIfItDoesntExist()
        {
            // Arrange
            if (Directory.Exists(DEFAULT_NEW_PROJECT_NAME))
            {
                Directory.Delete(DEFAULT_NEW_PROJECT_NAME);
            }
            
            // Act
            Program.Main(new string[0]);
            
            // Assert
            Assert.That(Directory.Exists(DEFAULT_NEW_PROJECT_NAME));
        }

        [Test]
        public void ProgramCreatesDirectoryUsingSpecifiedNameIfItDoesntExist()
        {
            const string projectName = "Asteroidz";

            // Arrange
            if (Directory.Exists(projectName))
            {
                Directory.Delete(projectName);
            }
            
            // Act
            Program.Main(new string[] { projectName });
            
            // Assert
            Assert.That(Directory.Exists(projectName));
        }
    }
}
