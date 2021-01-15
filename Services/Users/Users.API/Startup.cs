using AutoMapper;
using Users.API.Application.IntegrationEvents.EventHandling;
using Users.API.Application.IntegrationEvents.Events;
using Users.API.Application.Queries;
using Users.API.Infrastructure.Filters;
using Users.API.Services;
using Users.Domain.AggregateModel.UserAggregate;
using Users.Domain.SeedWork;
using Users.Infrastructure;
using Users.Infrastructure.Repositories;
using Users.Infrastructure.UnitOfWork;
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

namespace Users.API
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
                opt.Audience = "users";
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

            services.AddTransient<IUserQueries, UserQueries>();
            services.AddTransient<IFriendshipRequestQueries, FriendshipRequestQueries>();
            services.AddTransient<IFriendshipQueries, FriendshipQueries>();

            services.AddScoped<IUnitOfWork, SqlUnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IFriendshipRequestRepository, FriendshipRequestRepository>();
            services.AddScoped<IFriendshipRepository, FriendshipRepository>();

            services.AddEventBus(options =>
            {
                options.HostName = Configuration["EventBusConnection"];
                options.UserName = Configuration["EventBusUserName"];
                options.Password = Configuration["EventBusPassword"];
                options.QueueName = Configuration["EventBusQueueName"];
                options.RetryCount = 10;
            }, typeof(Startup).Assembly);

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IIdentityService, IdentityService>();

            services.AddHealthChecks()
                    .AddCheck("self-check", () => HealthCheckResult.Healthy(), tags: new[] { "self" })
                    .AddMySql(Configuration["ConnectionString"], name: "db-check", tags: new[] { "db" })
                    .AddRabbitMQ($"amqp://{Configuration["EventBusUserName"]}:{Configuration["EventBusPassword"]}@{Configuration["EventBusConnection"]}",
                        name: "rabbitmq-check", tags: new string[] { "rabbitmq" });

            services.AddAutoMapper(config =>
            {
                config.AddProfile(new AutoMapperConfig(new[] { typeof(Startup).Assembly }));
                config.CreateMap(typeof(JsonPatchDocument<>), typeof(JsonPatchDocument<>));
                config.CreateMap(typeof(Operation<>), typeof(Operation<>));
                config.CreateMap<FriendshipRequest, FriendshipRequest>();
                config.CreateMap<Friendship, Friendship>();
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
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseCors();

            app.UseAuthentication();

            app.UseMvc();

            app.ConfigureEventBus();
        }
    }

    internal static class CustomExtensionMethods
    {
        public static IApplicationBuilder ConfigureEventBus(this IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.Subscribe<UserAccountCreatedIntegrationEvent, UserAccountCreatedIntegrationEventHandler>();
            eventBus.Subscribe<UserConnectedIntegrationEvent, UserConnectedIntegrationEventHandler>();
            eventBus.Subscribe<UserDisconnectedIntegrationEvent, UserDisconnectedIntegrationEventHandler>();

            return app;
        }
    }
}
