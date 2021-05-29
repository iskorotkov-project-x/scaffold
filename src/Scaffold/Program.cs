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
using System.Text.RegularExpressions;

namespace Scaffold
{
    static class Program
    {
        private static readonly ServiceCollection _services = new ServiceCollection();
        private static ServiceProvider _serviceProvider;

        private static async Task<int> Main(string[] args)
        {
            string pathToTemplates = "";
            string pathToPlugins = "";
            try
            {
                pathToTemplates = Environment.GetEnvironmentVariable("SCAFFOLD_TEMPLATES");
                pathToPlugins = Environment.GetEnvironmentVariable("SCAFFOLD_PLUGINS");
            }
            catch (Exception) { }

            if (string.IsNullOrEmpty(pathToTemplates))
            {
                pathToTemplates = Path.Join(Environment.CurrentDirectory, "templates");
            }

            if (string.IsNullOrEmpty(pathToPlugins))
            {
                pathToTemplates = Path.Join(Environment.CurrentDirectory, "plugins");
            }

            if (!new DirectoryInfo(pathToTemplates).Exists)
            {
                Console.WriteLine($"Directory '{pathToTemplates}' not found!");
                Console.WriteLine($"Press any key to continue...");
                Console.ReadKey();
                return 0;
            }

            if (!new DirectoryInfo(pathToPlugins).Exists)
            {
                Console.WriteLine($"Directory '{pathToPlugins}' not found!");
                Console.WriteLine($"Press any key to continue...");
                Console.ReadKey();
                return 0;
            }



            // Add services. Services is used to take needed class with certain interface
            _services.AddSingleton<ILoader, FileSystemLoader>(x => new FileSystemLoader(pathToTemplates, pathToPlugins));
            _services.AddSingleton<IGenerator, LocalGenerator>();
            _services.AddSingleton<ITemplater, RegexTemplater>();

            _serviceProvider = _services.BuildServiceProvider();

            // Next we creating console commands
            // This is root console command whith option -l
            var rootCommand = new RootCommand
            {
                new Option<bool>(new[] {"-l", "--list"}, "Displays the entire list of templates"),
            };

            // Some argument need to be created outside '{...}'. This need for adding extra validation
            var createArgument = new Argument<string>("template-name", "The template used to create the project when executing the command");
            createArgument.AddValidator(cr =>
            {
                var loader = _serviceProvider.GetRequiredService<ILoader>();
                var templateInfos = loader.GetAllLanguagesAndTemplateNames().ToList();

                // check if we have such template
                if (!templateInfos.Select(x => x.Name).Contains(cr.Tokens[0].Value))
                {
                    return $"Sorry, but there is no '{cr.Tokens[0].Value}' template. To see entire list of templates please use 'scaffold -l'";
                }

                return null;
            });

            var languageOption = new Option<string>(new[] { "-lang", "--language" }, "The language of the template to create. Depends on the template") { IsRequired = true };
            languageOption.AddValidator(cr =>
            {
                var loader = _serviceProvider.GetRequiredService<ILoader>();
                var templateInfos = loader.GetAllLanguagesAndTemplateNames().ToList();

                // check if we have such language
                if (!templateInfos.Select(x => x.Language).Contains(cr.Tokens[0].Value))
                {
                    return $"Sorry, but there is no '{cr.Tokens[0].Value}' program language in template folder. To see entire list of templates please use 'scaffold -l'";
                }

                return null;
            });

            // This is 'create' console command for creating project from template
            var createCommand = new Command("create", "Create a project using the specified template")
            {
                createArgument,
                new Option<string>(new[] {"-n", "--name"},              "Sets the project name"),
                new Option<DirectoryInfo>(new[] { "-o", "--output" },   "Sets the location where the template will be created"),
                languageOption,
                new Option<string>(new[] {"-v", "--version"},           "Sets the SDK version"){ IsRequired = true },
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

            // Method is used for processing create command
            createCommand.Handler =
                CommandHandler
                    .Create<string, string, DirectoryInfo, string, string, bool, bool, bool, bool, bool, bool, bool, bool,
                        bool, bool, bool>(CreateCall);

            // Method is used for processing scaffold.exe call
            rootCommand.Handler = CommandHandler.Create<bool>(EmptyCall);

            // Adding createCommand to rootCommand
            rootCommand.AddCommand(createCommand);

            // If scaffold.exe calls without anything, need to show help
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
                Console.WriteLine($"{i + 1}    {templateInfos[i].Name}    {templateInfos[i].Language}");
            }
        }

        /// <summary>
        /// Create a project using the specified template. This method will be called when using the create command
        /// <param name="template"          >The template used to create the project when executing the command.</param>
        /// <param name="name"              >Sets the project name.</param>
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
        private static void CreateCall(string template, string name, DirectoryInfo output, string language, string version,
                                       bool git, bool docker, bool kubernetes, bool gitignore, bool dockerignore,
                                       bool readme, bool contributing, bool license, bool codeofproduct,
                                       bool githubworkflows, bool gitlabci)
        {
            // Comput output folder
            if (output != null)
            {
                if (!output.Exists)
                {
                    Console.WriteLine($"There is no directory '{output.FullName}'.");

                    // Wait to confirm creating directory
                    ConsoleKey response;
                    do
                    {
                        Console.Write("Create directory? [y/n] ");
                        response = Console.ReadKey(false).Key;
                        if (response != ConsoleKey.Enter)
                        {
                            Console.WriteLine();
                        }
                    } while (response != ConsoleKey.Y && response != ConsoleKey.N);

                    // If confirmed
                    if (response == ConsoleKey.Y)
                    {
                        try
                        {
                            output.Create();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Can`t create directory!");
                            Console.WriteLine(e.Message);
                            return;
                        }
                        if (!output.Exists)
                        {
                            Console.WriteLine("Can`t create directory!");
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Interrupted!");
                        return;
                    }
                }
            }
            else
            {
                // Standart directory - folder that hold scaffold.exe file
                output = new DirectoryInfo(Environment.CurrentDirectory);
            }

            // Computing name of the project
            if (string.IsNullOrEmpty(name))
            {
                name = "unnamed";
            }

            // Check if name is correct
            if (!Regex.IsMatch(name, @"^[a-zA-Z][\w]+$"))
            {
                Console.WriteLine($"Incorrect project name: '{name}'! Please use only Latin symbols, numbers and underscore");
                return;
            }

            // Check if there such directory compiled by output directory and project name
            var resultPath = Path.Join(output.FullName, name);
            if (new DirectoryInfo(resultPath).Exists)
            {
                int i = 0;
                // Folder must be unique
                do
                {
                    resultPath = Path.Join(output.FullName, $"{name}{++i}");
                } while (new DirectoryInfo(resultPath).Exists);
                 
                name = $"{name}{i}";
            }

            var pathToProject = Path.Join(output.FullName, name); 
            Console.WriteLine($"Project will be created in {pathToProject}.");
            Directory.CreateDirectory(pathToProject);



            var plugins = new List<string>();

            if (!string.IsNullOrEmpty(version))
            {
                Console.WriteLine($"The SDK version set to {version}.");
            }

            if (git)
            {
                plugins.Add(nameof(git));
                Console.WriteLine($"Added git support.");
            }

            if (docker)
            {
                plugins.Add(nameof(docker));
                Console.WriteLine($"Added Dockerfile support.");
            }

            if (kubernetes)
            {
                plugins.Add(nameof(kubernetes));
                Console.WriteLine($"Added Kubernetes support.");
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

            Console.WriteLine($"Creating project from '{template}' template.");

            Console.WriteLine($"Language is '{language}'.");

            // Get required services.
            var loader = _serviceProvider.GetRequiredService<ILoader>();
            var generator = _serviceProvider.GetRequiredService<IGenerator>();
            var templater = _serviceProvider.GetRequiredService<ITemplater>();

            var ctx = new Context() { Description = @"TODO description", ProjectName = name, Version = version };

            // Load template (with all files)
            var tl = loader.Load(language, template, plugins);
           
            // Generate project
            var notProcessedProject = generator.Generate(pathToProject, tl);

            // Replace {{ .<some> }} with things from context
            var project = templater.Substitute(ctx, notProcessedProject);

            //if (project.Compiles()) { 
            Console.WriteLine("Project crated successfully");
            Console.WriteLine($"Press any key to continue...");
            Console.ReadKey();
            //}
            //else
            //{
            //    Console.WriteLine("Project not compiled!");
            //}
        }
    }
}
