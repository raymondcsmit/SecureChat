using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Helpers.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Users.API.Infrastructure;

namespace Users.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var webHost = BuildWebHost(args)
                .MigrateDbContext<UsersDbContext>(services =>
                {
                    var usersDbContextSeed = services.GetRequiredService<UsersDbContextSeed>();
                    usersDbContextSeed.SeedAsync().Wait();
                });

            await webHost.RunAsync();
        }

        private static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration(ConfigureConfiguration)
                .ConfigureLogging(ConfigureLogger)
                .UseStartup<Startup>()
                .Build();

        private static void ConfigureConfiguration(WebHostBuilderContext ctx, IConfigurationBuilder config) =>
            config.SetBasePath(ctx.HostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{ctx.HostingEnvironment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

        private static void ConfigureLogger(WebHostBuilderContext ctx, ILoggingBuilder logging) =>
            logging.AddConfiguration(ctx.Configuration.GetSection("Logging"))
                .AddConsole()
                .AddDebug();
    }
}
