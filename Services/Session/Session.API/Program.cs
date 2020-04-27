using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Session.API.Infrastructure;
using Helpers.Extensions;

namespace Session.API
{
    public class Program
    {
        public const string AppName = "Session";

        public static async Task Main(string[] args)
        {
            var webHost = BuildWebHost(args)
                .MigrateDbContext<SessionContext>();
            await webHost.RunAsync();
        }

        private static IWebHost BuildWebHost(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .CaptureStartupErrors(false)
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureAppConfiguration(ConfigureConfiguration)
            .ConfigureLogging(ConfigureLogger)
            .UseStartup<Startup>()
            .Build();

        private static void ConfigureConfiguration(WebHostBuilderContext ctx, IConfigurationBuilder config) =>
            config.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{ctx.HostingEnvironment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

        private static void ConfigureLogger(WebHostBuilderContext ctx, ILoggingBuilder logging) =>
            logging.AddConfiguration(ctx.Configuration.GetSection("Logging"))
                .AddConsole()
                .AddDebug();

    }
}
