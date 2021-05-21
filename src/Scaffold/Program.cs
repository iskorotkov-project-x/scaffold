using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Scaffold
{
    static class Program
    {
        private static async Task<int> Main(string[] args)
        {
            var services = new ServiceCollection();

            // Add services.
            // services.AddScoped<InterfaceType, RealType>();

            await using var serviceProvider = services.BuildServiceProvider();

            // Get required services.
            // var v = serviceProvider.GetRequiredService<InterfaceType>();

            var rootCommand = new RootCommand
            {
                new Option<bool>(new[] {"-l", "--list"}, "Displays the entire list of templates"),
            };

            var createCommand = new Command("create", "Create a project using the specified template")
            {
                new Option<bool>(new[] {"-?", "-h", "--help"}, "Show help and usage information"),
                new Option<string>(new[] {"-n", "--name"}, "Sets the name of the output data"),
                new Option<DirectoryInfo>(new[] {"-o", "--output"},
                                          "Sets the location where the template will be created"),
                // TODO: Надо посмотреть как сделать Required Option
                new Option<string>(new[] {"-lang", "--language"},
                                   "The language of the template to create. Depends on the template"),
                new Option<string>(new[] {"-v", "--version"}, "Sets the SDK version"),
                new Option<bool>(new[] {"--git"}, "Adds Git support"),
                new Option<bool>(new[] {"--docker"}, "Adds Dockerfile support"),
                new Option<bool>(new[] {"-kn", "--kubernetes"}, "Adds Kubernetes support"),
                new Option<bool>(new[] {"-gi", "--gitignore"}, "Adds a file .gitignore"),
                new Option<bool>(new[] {"-di", "--dockerignore"}, "Adds a file .dockerignore"),
                new Option<bool>(new[] {"-rm", "--readme"}, "Adds a file README.md"),
                new Option<bool>(new[] {"-con", "--contributing"}, "Adds a file CONTRIBUTING.md"),
                new Option<bool>(new[] {"-li", "--license"}, "Adds a file LICENSE"),
                new Option<bool>(new[] {"-cop", "--codeofproduct"}, "Adds a file CODE_OF_PRODUCT.md"),
                new Option<bool>(new[] {"-ghw", "--githubworkflows"}, "Adds files for GitHub Workflows"),
                new Option<bool>(new[] {"-glci", "--gitlabci"}, "Adds files for GitLab CI"),
            };

            createCommand.Handler =
                CommandHandler
                    .Create<bool, string, DirectoryInfo, string, string, bool, bool, bool, bool, bool, bool, bool, bool,
                        bool, bool, bool>(CreateCall);
            rootCommand.AddCommand(createCommand);

            rootCommand.Handler = CommandHandler.Create<bool>(EmptyCall);

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
            Console.WriteLine("n    Name            Languages");
            Console.WriteLine("1    template1Name   C#, go");
            Console.WriteLine("2    template2Name   C#, go");
            Console.WriteLine("3    template3Name   C#");
            Console.WriteLine("4    template4Name   C#");
            Console.WriteLine("5    template5Name   go");
        }

        /// <summary>
        /// Create a project using the specified template
        /// <param name="help"              >Show help and usage information.</param>
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
        private static void CreateCall(bool help, string name, DirectoryInfo output, string language, string version,
                                       bool git, bool docker, bool kubernetes, bool gitignore, bool dockerignore,
                                       bool readme, bool contributing, bool license, bool codeofproduct,
                                       bool githubworkflows, bool gitlabci)
        {
            if (help)
            {
                ShowHelpCreate();
                return;
            }

            if (output != null)
            {
                if (!output.Exists)
                {
                    Console.WriteLine($"OUTPUT_DIRECTORY \"{output}\" does not exist. Please set correct folder.");
                    return;
                }

                Console.WriteLine($"OUTPUT_DIRECTORY set to \"{output}\".");
            }

            if (!string.IsNullOrEmpty(name))
            {
                Console.WriteLine($"The name of the output data set to {name}.");
            }

            if (string.IsNullOrEmpty(language) || !new[] { "C#", "csharp", "go" }.Contains(language))
            {
                Console.WriteLine($"Please enter correct language:");
                foreach (var lng in new[] { "C#", "csharp", "go" })
                {
                    Console.WriteLine(lng);
                }
                return;
            }

            Console.WriteLine($"Creating project from {language} template.");

            if (!string.IsNullOrEmpty(version))
            {
                Console.WriteLine($"The SDK version set to {version}.");
            }

            if (git)
            {
                Console.WriteLine($"Added git support.");
            }

            if (docker)
            {
                Console.WriteLine($"Added Dockerfile support.");
            }

            if (kubernetes)
            {
                Console.WriteLine($"Added Kubernetes support.");
            }

            if (gitignore)
            {
                Console.WriteLine($"Added .gitignore file.");
            }

            if (dockerignore)
            {
                Console.WriteLine($"Added .dockerignore file.");
            }
            if (readme)
            {
                Console.WriteLine($"Added README.md file.");
            }

            if (contributing)
            {
                Console.WriteLine($"Added CONTRIBUTING.md file.");
            }

            if (license)
            {
                Console.WriteLine($"Added LICENSE file.");
            }

            if (codeofproduct)
            {
                Console.WriteLine($"Added CODE_OF_PRODUCT.md file.");
            }

            if (githubworkflows)
            {
                Console.WriteLine($"Added files for GitHub Workflows.");
            }

            if (gitlabci)
            {
                Console.WriteLine($"Added files for GitLab CI.");
            }
        }

        /// <summary>
        /// Show help of command "create"
        /// </summary>
        private static void ShowHelpCreate()
        {
            Console.WriteLine(
                @"
Create a project using the specified template
usage: scaffold create <TEMPLATE> [<options>]

Arguments:
     TEMPLATE	                The template used to create the project when executing the command
Options:	
     -n | --name <OUTPUT_NAME>
                                Sets the name of the output data
     -o | --output <OUTPUT_DIRECTORY>
                                Sets the location where the template will be created
     -lang | --language {[""C#"" | csharp] | go}
                                The language of the template to create.Depends on the template
     - v | --version < VERSION_NUMBER >
                                Sets the SDK version
     --git                      Adds Git support
     --docker                   Adds Dockerfile support
     - kn | --kubernetes        Adds Kubernetes support
     - gi | --gitignore         Adds a file.gitignore
     - di | --dockerignore      Adds a file.dockerignore
     - rm | --readme            Adds a file README.md
     - con | --contributing     Adds a file CONTRIBUTING.md
     - li | --license           Adds a file LICENSE
     - cop | --codeofproduct    Adds a file CODE_OF_PRODUCT.md
     - ghw | --githubworkflows  Adds files for GitHub Workflows
     - glci | --gitlabci        Adds files for GitLab CI

");
        }
    }
}