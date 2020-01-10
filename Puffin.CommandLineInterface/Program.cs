using System;
using System.IO;

namespace Puffin.CommandLineInterface
{
    class Program
    {
        private const string DEFAULT_NEW_PROJECT_NAME = "NewPuffin";

        static void Main(string[] args)
        {
            // TODO: don't assume the request is a new project
            var projectName = args.Length == 1 ? args[0] : DEFAULT_NEW_PROJECT_NAME;

            if (Directory.Exists(projectName))
            {
                Console.WriteLine($"Can't create a new project directory named {projectName}, the directory already exists.");
            }
            else
            {
                Directory.CreateDirectory(projectName);
                Console.WriteLine($"Created a new project in the {projectName} directory!");
            }
            
        }
    }
}
