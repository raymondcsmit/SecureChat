using System;
using System.Net.Http;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Polly;
using Polly.Extensions.Http;
using Registration.Services;

namespace Registration
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks(Configuration);

            services.AddMvc()
                .AddSessionStateTempDataProvider();

            services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
            });
            services.AddHttpContextAccessor();

            services.AddHttpClient<IUsersClient, UsersClient>(config =>
                    {
                        config.BaseAddress = new Uri(Configuration["UsersApiUrl"]);
                    })
                .AddTransientHttpErrorPolicy(builder => builder
                    .WaitAndRetryAsync(retryCount: 3, sleepDurationProvider: retryAttempt => retryAttempt * TimeSpan.FromSeconds(3)));


            services.AddTransient<ILoginUrlService, LoginUrlService>();
            services.AddScoped<IUsersClient, UsersClient>();
            services.AddTransient<IActionUrlGeneratorService, ActionActionUrlGeneratorService>();
            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
            services.AddHttpClientServices(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHealthChecks("/hc", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseSession();
            app.UseMvc();
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy());
                //.AddUrlGroup(new Uri(configuration["UsersUrlHC"]), name: "usersapigw-check", tags: new string[] { "usersapigw" });

            return services;
        }

        public static IServiceCollection AddHttpClientServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IUsersClient, UsersClient>()
                    .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Sample. Default lifetime is 2 minutes
                   .AddPolicyHandler(GetRetryPolicy())
                   .AddPolicyHandler(GetCircuitBreakerPolicy());

            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        }

        private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        }
    }
}
