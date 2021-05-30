using Generator.Local;
using Loader.FileSystem;
using Model;
using System;
using System.Collections.Generic;
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
            var loader = new FileSystemLoader(Environment.GetEnvironmentVariable("SCAFFOLD_TEMPLATES"), Environment.GetEnvironmentVariable("SCAFFOLD_PLUGINS"));
            
            var pluginsName = new string[] {"docker", "kubernetes"};
            var tl = loader.Load("c#", "console", pluginsName);
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
                Description = "test c# project",
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
