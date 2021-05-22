using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Business;
using Generator.Local;
using Loader.FileSystem;
using Microsoft.Extensions.DependencyInjection;
using Model;
using Templater.Regex;

namespace Scaffold
{
    static class Program
    {
        static List<Template> templates = new List<Template>()
        {
            new Template() { TemplateName = "tl1", Language="c#" },
            new Template() { TemplateName = "tl2", Language="go" },
            new Template() { TemplateName = "tl3", Language="go" },
            new Template() { TemplateName = "tl4", Language="c#" },
        };
        private static readonly ServiceCollection _services = new ServiceCollection();
        private static ServiceProvider _serviceProvider;

        private static async Task<int> Main(string[] args)
        {
            // Add services.
            _services.AddSingleton<ILoader, FileSystemLoader>(x => new FileSystemLoader("C:\\templates"));
            _services.AddSingleton<IGenerator, LocalGenerator>();
            _services.AddSingleton<ITemplater, RegexTemplater>();

            _serviceProvider = _services.BuildServiceProvider();

            // Get required services.
            // var v = serviceProvider.GetRequiredService<InterfaceType>();

            var rootCommand = new RootCommand
            {
                new Option<bool>(new[] {"-l", "--list"}, "Displays the entire list of templates"),
            };

            var createCommand = new Command("create", "Create a project using the specified template")
            {
                new Argument<Template>("template-name", "The template used to create the project when executing the command").FromAmong(templates.Select(x => x.TemplateName).ToArray()),
                new Option<string>(new[] {"-n", "--name"},              "Sets the name of the output data"),
                new Option<DirectoryInfo>(new[] { "-o", "--output" },   "Sets the location where the template will be created").ExistingOnly(),
                new Option<string>(new[] { "-lang", "--language" },     "The language of the template to create. Depends on the template") { IsRequired = true }.FromAmong("c#", "C#", "csharp", "go", "GO"),
                new Option<string>(new[] {"-v", "--version"},           "Sets the SDK version"),
                new Option<bool>(new[] {"--git"},                       "Adds Git support"),
                new Option<bool>(new[] {"--docker"},                    "Adds Dockerfile support"),
                new Option<bool>(new[] {"-kn", "--kubernetes"},         "Adds Kubernetes support"),
                new Option<bool>(new[] {"-gi", "--gitignore"},          "Adds a file .gitignore"),
                new Option<bool>(new[] {"-di", "--dockerignore"},       "Adds a file .dockerignore"),
                new Option<bool>(new[] {"-rm", "--readme"},             "Adds a file README.md"),
                new Option<bool>(new[] {"-con", "--contributing"},      "Adds a file CONTRIBUTING.md"),
                new Option<bool>(new[] {"-li", "--license"},            "Adds a file LICENSE"),
                new Option<bool>(new[] {"-cop", "--codeofproduct"},     "Adds a file CODE_OF_PRODUCT.md"),
                new Option<bool>(new[] {"-ghw", "--githubworkflows"},   "Adds files for GitHub Workflows"),
                new Option<bool>(new[] {"-glci", "--gitlabci"},         "Adds files for GitLab CI"),
            };

            createCommand.Handler =
                CommandHandler
                    .Create<Template, string, DirectoryInfo, string, string, bool, bool, bool, bool, bool, bool, bool, bool,
                        bool, bool, bool>(CreateCall);

            rootCommand.Handler = CommandHandler.Create<bool>(EmptyCall);

            rootCommand.AddCommand(createCommand);

            if (args.Length == 0)
            {
                return await rootCommand.InvokeAsync(new[] { "-h" });
            }

            return await rootCommand.InvokeAsync(args);
        }

        /// <summary>
        /// Root command executor handler
        /// </summary>
        /// <param name="list">Displays the entire list of templates.</param>
        private static void EmptyCall(bool list)
        {
            if (list)
            {
                ShowListTemplates();
            }
        }

        /// <summary>
        /// Displays the entire list of templates    
        /// </summary>
        private static void ShowListTemplates()
        {
            var loader = _serviceProvider.GetRequiredService<ILoader>();
            var templateInfos = loader.GetAllLanguagesAndTemplateNames().ToList();
            Console.WriteLine("n    Name   Languages");
            for (int i = 0; i < templateInfos.Count(); i++)
            {
                Console.WriteLine($"{i + 1}    {templateInfos[i].TemplateName}    {templateInfos[i].Language}");
            }
        }

        /// <summary>
        /// Create a project using the specified template
        /// <param name="template"          >The template used to create the project when executing the command.</param>
        /// <param name="name"              >Sets the name of the output data.</param>
        /// <param name="output"            >Sets the location where the template will be created.</param>
        /// <param name="language"          >The language of the template to create. Depends on the template.</param>
        /// <param name="version"           >Sets the SDK version.</param>
        /// <param name="git"               >Adds Git support.</param>
        /// <param name="docker"            >Adds Dockerfile support.</param>
        /// <param name="kubernetes"        >Adds Kubernetes support.</param>
        /// <param name="gitignore"         >Adds a file .gitignore.</param>
        /// <param name="dockerignore"      >Adds a file .dockerignore.</param>
        /// <param name="readme"            >Adds a file README.md.</param>
        /// <param name="contributing"      >Adds a file CONTRIBUTING.md.</param>
        /// <param name="license"           >Adds a file LICENSE.</param>
        /// <param name="codeofproduct"     >Adds a file CODE_OF_PRODUCT.md.</param>
        /// <param name="githubworkflows"   >Adds files for GitHub Workflows.</param>
        /// <param name="gitlabci"          >Adds files for GitLab CI.</param>
        /// </summary>
        private static void CreateCall(Template template, string name, DirectoryInfo output, string language, string version,
                                       bool git, bool docker, bool kubernetes, bool gitignore, bool dockerignore,
                                       bool readme, bool contributing, bool license, bool codeofproduct,
                                       bool githubworkflows, bool gitlabci)
        {
            Console.WriteLine($"Creating project from {template.TemplateName} template.");

            Console.WriteLine($"Language is {language}.");

            if (output != null)
            {
                Console.WriteLine($"TODO Output directory set to \"{output}\".");
            }

            if (!string.IsNullOrEmpty(name))
            {
                Console.WriteLine($"TODO The name of the output data set to {name}.");
            }

            if (!string.IsNullOrEmpty(version))
            {
                Console.WriteLine($"TODO The SDK version set to {version}.");
            }

            if (git)
            {
                Console.WriteLine($"TODO Added git support.");
            }

            if (docker)
            {
                Console.WriteLine($"TODO Added Dockerfile support.");
            }

            if (kubernetes)
            {
                Console.WriteLine($"TODO Added Kubernetes support.");
            }

            if (gitignore)
            {
                Console.WriteLine($"TODO Added .gitignore file.");
            }

            if (dockerignore)
            {
                Console.WriteLine($"TODO Added .dockerignore file.");
            }

            if (readme)
            {
                Console.WriteLine($"TODO Added README.md file.");
            }

            if (contributing)
            {
                Console.WriteLine($"TODO Added CONTRIBUTING.md file.");
            }

            if (license)
            {
                Console.WriteLine($"TODO Added LICENSE file.");
            }

            if (codeofproduct)
            {
                Console.WriteLine($"TODO Added CODE_OF_PRODUCT.md file.");
            }

            if (githubworkflows)
            {
                Console.WriteLine($"TODO Added files for GitHub Workflows.");
            }

            if (gitlabci)
            {
                Console.WriteLine($"TODO Added files for GitLab CI.");
            }

            var loader = _serviceProvider.GetRequiredService<ILoader>();
            // var generator = serviceProvider.GetRequiredService<>();
            // var templater = serviceProvider.GetRequiredService<>();
            var templateInfos = loader.GetAllLanguagesAndTemplateNames().ToList();
            Console.WriteLine("n    Name   Languages");
            for (int i = 0; i < templateInfos.Count(); i++)
            {
                Console.WriteLine($"{i + 1}    {templateInfos[i].TemplateName}    {templateInfos[i].Language}");
            }

            //var template = loader.Load(language, )
        }
    }
}
