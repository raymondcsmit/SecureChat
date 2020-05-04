using AutoMapper;
using HealthChecks.UI.Client;
using Helpers.Mapping;
using Messaging.Hubs;
using Messaging.IntegrationEvents.EventHandling;
using Messaging.IntegrationEvents.Events;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SecureChat.Common.Events.EventBus.Abstractions;
using SecureChat.Common.Events.EventBusRabbitMQ.Extensions;
using System.Threading.Tasks;

namespace Messaging
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
            services.AddControllers();

            services.AddCors(options =>
             {
                 options.AddPolicy("CorsPolicy",
                     builder => builder
                     .AllowAnyMethod()
                     .AllowAnyHeader()
                     .SetIsOriginAllowed((host) => true)
                     .AllowCredentials());
             });

            AddAuthService(services);

            AddHealthChecks(services);

            AddEventBus(services);

            services.AddAutoMapper(config => 
                config.AddProfile(new AutoMapperConfig(new[] { typeof(Startup).Assembly })), typeof(Startup).Assembly);

            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("CorsPolicy");

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                {
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHub<MessagingHub>("/hub/messaginghub", options => options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransports.All);
            });

            ConfigureEventBus(app);
        }

        private void AddAuthService(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                // Identity made Cookie authentication the default.
                // However, we want JWT Bearer Auth to be the default.
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = Configuration["AuthUrl"];
                options.RequireHttpsMetadata = false;
                options.Audience = "messaging";
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/hub/messaginghub")))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        }
        
        private void AddHealthChecks(IServiceCollection services)
        {
            services.AddHealthChecks()
                    .AddCheck("self-check", () => HealthCheckResult.Healthy(), tags: new[] { "self" })
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
            eventBus.Subscribe<FriendshipRequestCreatedIntegrationEvent, FriendshipRequestCreatedIntegrationEventHandler>();
            eventBus.Subscribe<FriendshipCreatedIntegrationEvent, FriendshipCreatedIntegrationEventHandler>();
            eventBus.Subscribe<NotifyUsersIntegrationEvent, NotifyUsersIntegrationEventHandler>();
        }
    }
}
