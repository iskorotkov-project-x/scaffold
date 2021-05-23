using System;
using System.Collections.Generic;
using System.IO;
using Generator.Local;
using Loader.FileSystem;
using Model;
using Xunit;

namespace Generator.Tests
{
    public class LocalGeneratorTests
    {
        [Fact]
        public void GenerateProject()
        {
            // Arrange
            var loader = new FileSystemLoader(Environment.GetEnvironmentVariable("SCAFFOLD_TEMPLATES"));
            var tl = loader.Load("c#", "console");
            var generator = new LocalGenerator();

            var di = new DirectoryInfo($"{Environment.CurrentDirectory}/testProject");
            if (di.Exists)
            {
                di.Delete(true);
            }

            // Act
            var project = generator.Generate($"{Environment.CurrentDirectory}/testProject", tl);

            // Assert
            Assert.NotEmpty(project.CreatedFiles);
        }
    }
}
