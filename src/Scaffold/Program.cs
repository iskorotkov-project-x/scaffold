using System;
using System.CommandLine;
using System.CommandLine.Invocation;
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

            var rootCommand = new RootCommand("some description");
            rootCommand.AddOption(new Option<int>(new[] { "--num", "-n" }, () => 42, "option description"));
            rootCommand.Handler = CommandHandler.Create<int>(PrintNum);

            return await rootCommand.InvokeAsync(args);
        }

        private static void PrintNum(int num)
        {
            Console.WriteLine($"You entered {num}");
        }
    }
}
