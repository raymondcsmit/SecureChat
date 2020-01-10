using AutoMapper;
using Chat.API.Application.IntegrationEvents.EventHandling;
using Chat.API.Application.IntegrationEvents.Events;
using Chat.API.Infrastructure;
using Chat.API.Infrastructure.Filters;
using Chat.API.Services;
using Chat.Domain.AggregateModel.UserAggregate;
using Chat.Domain.SeedWork;
using Chat.Infrastructure;
using Chat.Infrastructure.HealthChecks;
using Chat.Infrastructure.Repositories;
using Chat.Infrastructure.UnitOfWork;
using HealthChecks.UI.Client;
using Helpers.Auth;
using Helpers.Extensions;
using Helpers.Mapping;
using Helpers.Resilience;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SecureChat.Common.Events.EventBus.Abstractions;
using SecureChat.Common.Events.EventBusRabbitMQ.Extensions;
using Steeltoe.Discovery.Client;

namespace Chat.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;
        }

        IConfiguration Configuration { get; }
        IHostingEnvironment HostingEnvironment { get; }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddDiscoveryClient(Configuration);

            services.AddMvc(options =>
            {
                //var policy = new AuthorizationPolicyBuilder()
                //    .RequireAuthenticatedUser()
                //    .Build();
                //options.Filters.Add(new AuthorizeFilter(policy));
                options.Filters.Add(typeof(GlobalExceptionFilter));
            })
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });

            services.AddAuthenticationWithBypass(opt =>
            {
                opt.Authority = Configuration["AuthUrl"];
                opt.Audience = "chat";
                opt.RequireHttpsMetadata = false;
            }, opt =>
            {
                opt.BypassAuthenticationHeader = Configuration["BypassAuthenticationHeader"];
                opt.BypassAuthenticationSecret = Configuration["BypassAuthenticationSecret"];
            });

            // No need for origin restrictions, since the microservice will not be exposed externally
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            services.Configure<DbConnectionInfo>(Configuration);
            services.AddTransient<IDatabaseResiliencePolicy, DatabaseResiliencePolicy>();
            services.AddScoped<IDbConnectionFactory, ResilientMySqlConnectionFactory>();
            services.AddScoped<IUnitOfWork, SqlUnitOfWork>();

            services.AddScoped<IUserRepository, UserRepository>();

            services.AddEventBus(options =>
            {
                options.HostName = Configuration["EventBusConnection"];
                options.UserName = Configuration["EventBusUserName"];
                options.Password = Configuration["EventBusPassword"];
                options.QueueName = Configuration["EventBusQueueName"];
            }, typeof(Startup).Assembly);

            services.AddScoped<DatabaseSeed>();

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IIdentityService, IdentityService>();

            services.AddHealthChecks()
                .AddCheck("self-check", () => HealthCheckResult.Healthy())
                .AddCheck("db-check", new MySqlConnectionHealthCheck(Configuration["ConnectionString"]));

            services.AddAutoMapper(config =>
            {
                config.AddProfile(new AutoMapperConfig(new[] { typeof(Startup).Assembly }));
                config.CreateMap(typeof(JsonPatchDocument<>), typeof(JsonPatchDocument<>));
                config.CreateMap(typeof(Operation<>), typeof(Operation<>));
                config.ShouldMapProperty = p => p.GetMethod.IsPublic || p.GetMethod.IsAssembly;
            }, typeof(Startup).Assembly);

            services.AddMediatR(typeof(Startup).Assembly);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }

            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self-check"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseHealthChecks("/readiness", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("db-check"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseCors();

            app.UseAuthentication();

            app.UseMvc();

            app.UseDiscoveryClient();

            app.ConfigureEventBus();
        }
    }

    internal static class CustomExtensionMethods
    {
        public static IApplicationBuilder ConfigureEventBus(this IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.Subscribe<UserAccountCreatedIntegrationEvent, UserAccountCreatedIntegrationEventHandler>();

            return app;
        }
    }
}
