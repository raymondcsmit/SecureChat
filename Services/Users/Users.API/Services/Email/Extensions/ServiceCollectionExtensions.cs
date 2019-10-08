using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Users.API.Services.Email.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEmailSender(this IServiceCollection services, IConfiguration configuration, IHostingEnvironment environment)
        {
            services.Configure<EmailSenderOptions>(configuration);
            if (environment.IsDevelopment())
            {
                services.AddScoped<IEmailSender, DebugEmailSender>();
            }
            else
            {
                services.AddScoped<IEmailSender, DefaultEmailSender>();
            }
            return services;
        }
    }
}
