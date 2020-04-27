using System;
using System.Threading.Tasks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using SecureChat.Common.Events.EventBus.Abstractions;
using SecureChat.Common.Events.EventBusRabbitMQ.Extensions;
using Session.API.Infrastructure;
using Session.API.Services;

namespace Session.API
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

            AddHealthChecks(services);

            AddAuth(services);

            AddEventBus(services);

            services.AddDbContext<SessionContext>(options =>
            {
                options.UseMySql(Configuration["ConnectionString"],
                    opt =>
                    {
                        opt.MigrationsAssembly(typeof(Startup).Assembly.FullName);
                        opt.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: Array.Empty<int>());
                    });
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .SetIsOriginAllowed((host) => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services.AddTransient<IIdentityService, IdentityService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("CorsPolicy");

            app.UseRouting();

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
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = Configuration["AuthUrl"];
                options.RequireHttpsMetadata = false;
                options.Audience = "messaging";
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
            //eventBus.Subscribe<FriendshipRequestMadeIntegrationEvent, FriendshipRequestMadeIntegrationEventHandler>();
        }
    }
}
