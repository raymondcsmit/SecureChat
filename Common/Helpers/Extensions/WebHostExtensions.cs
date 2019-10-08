using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;

namespace Helpers.Extensions
{
    public static class WebHostExtensions
    {
        public static IWebHost MigrateDbContext<TContext>(this IWebHost webHost, Action<IServiceProvider> action = null) where TContext : DbContext
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var context = serviceProvider.GetRequiredService<TContext>();
                var logger = serviceProvider.GetRequiredService<ILogger<TContext>>();
                
                var retryPolicy = Policy.Handle<Exception>()
                    .WaitAndRetry(
                        retryCount: 5,
                        sleepDurationProvider: retry => TimeSpan.FromSeconds(5 * retry)
                    );

                retryPolicy.Execute(() =>
                {
                    context.Database.Migrate();
                    action?.Invoke(serviceProvider);
                });

                logger.LogInformation($"Migrated database associated with context {typeof(TContext).Name}");
            }
            return webHost;
        }
    }
}
