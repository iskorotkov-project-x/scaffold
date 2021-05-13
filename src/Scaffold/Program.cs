using System;
using Microsoft.Extensions.DependencyInjection;

namespace Scaffold
{
    static class Program
    {
        private static void Main()
        {
            var services = new ServiceCollection();
            
            // Add services.
            // services.AddScoped<InterfaceType, RealType>();
            
            using var serviceProvider = services.BuildServiceProvider();
            
            // Get required services.
            // var v = serviceProvider.GetRequiredService<InterfaceType>();
            
            Console.WriteLine("Hello World!");
        }
    }
}
