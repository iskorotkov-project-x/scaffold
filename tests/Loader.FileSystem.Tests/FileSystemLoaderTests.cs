using Loader.FileSystem;
using System;
using System.Collections.Generic;
using Xunit;

namespace Loader.Tests
{
    public class FileSystemLoaderTests
    {
        [Fact]
        public void LoadTemplate()
        {
            // Arrange
            var loader = new FileSystemLoader(Environment.GetEnvironmentVariable("SCAFFOLD_TEMPLATES"), Environment.GetEnvironmentVariable("SCAFFOLD_PlUGINS"));    

            // Act
            var pluginsName = new string[] { "docker", "kubernetes" };
            var template = loader.Load("c#", "console", pluginsName);
            
            // Assert
            Assert.NotNull(template);
            Assert.Equal("c#", template.Language);
            Assert.Equal("console", template.Name);
            Assert.NotEmpty(template.Files);

            // in current state we not use plugins
            //Assert.NotEmpty(template.Plugins);
        }
    }
}
