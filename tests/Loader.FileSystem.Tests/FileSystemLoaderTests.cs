using Loader.FileSystem;
using System;
using Xunit;

namespace Loader.Tests
{
    public class FileSystemLoaderTests
    {
        [Fact]
        public void LoadTemplate()
        {
            var loader = new FileSystemLoader($"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\.scaffold\\templates");
            var template = loader.Load("c#", "tl1");

            Assert.NotNull(template);
            Assert.Equal("c#", template.Language);
            Assert.Equal("tl1", template.Name);
            Assert.NotEmpty(template.Files);

            // in curent state we not use plugins
            //Assert.NotEmpty(template.Plugins);
        }
    }
}
