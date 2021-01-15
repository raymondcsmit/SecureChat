using System;
using AutoMapper;
using Chats.Api.Application.Queries;
using Chats.Api.Extensions;
using Chats.Api.Infrastructure.Filters;
using Chats.Api.Mapping;
using Chats.Api.Services;
using Chats.Domain.AggregateModel;
using Chats.Domain.SeedWork;
using Chats.Infrastructure;
using Chats.Infrastructure.Repositories;
using HealthChecks.UI.Client;
using Helpers.Auth;
using Helpers.Mapping;
using Helpers.Resilience;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SecureChat.Common.Events.EventBus.Abstractions;
using SecureChat.Common.Events.EventBusRabbitMQ.Extensions;
using Users.API.Client.Extensions;

namespace Chats.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
                {
                    options.Filters.Add(typeof(GlobalExceptionFilter));
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });

            AddHealthChecks(services);

            AddAuth(services);

            AddEventBus(services);

            services.AddDbContext<ChatsContext>(options =>
            {
                options.UseMySql(Configuration["ConnectionString"],
                    opt =>
                    {
                        opt.MigrationsAssembly(typeof(ChatsContext).Assembly.FullName);
                        //opt.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: Array.Empty<int>());
                    });
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddScoped<IGenericRepository<Chat>, GenericRepository<Chat>>();

            services.AddScoped<IChatQueries, ChatQueries>();

            services.AddUsersApiClient(Configuration);

            services.AddMediatR(typeof(Startup).Assembly);
            services.AddAutoMapper(config =>
            {
                config.AddProfile(new AutoMapperConfig(new[] { typeof(Startup).Assembly }));
                config.AddProfile(new ChatMappingProfile());
            }, typeof(Startup).Assembly);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                {
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });

            });

            ConfigureEventBus(app);
        }

        private void AddAuth(IServiceCollection services)
        {
            services.AddAuthenticationWithBypass(opt =>
            {
                opt.Authority = Configuration["AuthUrl"];
                opt.Audience = "chats";
                opt.RequireHttpsMetadata = false;
            }, opt =>
            {
                opt.BypassAuthenticationHeader = Configuration["BypassAuthenticationHeader"];
                opt.BypassAuthenticationSecret = Configuration["BypassAuthenticationSecret"];
            });
        }

        private void AddHealthChecks(IServiceCollection services)
        {
            services.AddHealthChecks()
                    .AddCheck("self-check", () => HealthCheckResult.Healthy(), tags: new[] { "self" })
                    .AddMySql(Configuration["ConnectionString"], name: "db-check", tags: new[] { "db" })
                    .AddRabbitMQ($"amqp://{Configuration["EventBusUserName"]}:{Configuration["EventBusPassword"]}@{Configuration["EventBusConnection"]}",
                        name: "rabbitmq-check", tags: new string[] { "rabbitmq" });
        }

        private void AddEventBus(IServiceCollection services)
        {
            services.AddEventBus(options =>
            {
                options.HostName = Configuration["EventBusConnection"];
                options.UserName = Configuration["EventBusUserName"];
                options.Password = Configuration["EventBusPassword"];
                options.QueueName = Configuration["EventBusQueueName"];
                options.RetryCount = 10;
            }, typeof(Startup).Assembly);
        }

        public void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            //eventBus.Subscribe<UserConnectedIntegrationEvent, UserConnectedIntegrationEventHandler>();
        }
    }
}
