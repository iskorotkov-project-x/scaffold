using Generator.Local;
using Loader.FileSystem;
using Model;
using System;
using System.IO;
using Templater.Regex;
using Xunit;

namespace Templater.Tests
{
    public class RegexTemplaterTests
    {
        [Fact]
        public void SubstituteTemplates()
        {
            // Arrange
            var loader = new FileSystemLoader($"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\.scaffold\\templates");
            var tl = loader.Load("c#", "tl1");
            var generator = new LocalGenerator();

            var di = new DirectoryInfo($"{Environment.CurrentDirectory}/testProject");
            if (di.Exists)
            {
                di.Delete(true);
            }

            var project = generator.Generate($"{Environment.CurrentDirectory}/testProject", tl);

            var templater = new RegexTemplater();

            // Act
            var templatedProject = templater.Substitute(new Context {
                ProjectName = "testProject",
                Description = "тестовый c# проект",
                Version = "net5.0",
            }, project);

            // Assert
            Assert.NotEmpty(templatedProject.ProcessedFiles);
            //Assert.True(project.Compiles());

            foreach (var file in templatedProject.ProcessedFiles)
            {
                Assert.False(file.ContainsTemplates());
            }
        }
    }
}
