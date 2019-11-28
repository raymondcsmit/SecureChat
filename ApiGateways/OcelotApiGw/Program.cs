using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace OcelotApiGw
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var builder = WebHost.CreateDefaultBuilder(args);
            builder.ConfigureServices(s => s.AddSingleton(builder))
                .ConfigureAppConfiguration(ic => ic.AddJsonFile(Path.Combine("configuration", "configuration.json")))
                .UseStartup<Startup>()
                .ConfigureLogging((context, config) =>
                {
                    config.AddConfiguration(context.Configuration.GetSection("Logging"));
                    config.AddConsole();
                    config.AddDebug();
                });

            var host = builder.Build();
            return host;
        }
    }
}
