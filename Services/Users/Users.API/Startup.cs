using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AutoMapper;
using HealthChecks.UI.Client;
using Helpers.Extensions;
using Helpers.Mapping;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SecureChat.Common.Events.EventBusRabbitMQ.Extensions;
using Steeltoe.Common.Discovery;
using Steeltoe.Discovery.Client;
using Users.API.Application.Queries;
using Users.API.Infrastructure;
using Users.API.Infrastructure.Filters;
using Users.API.Infrastructure.HealthChecks;
using Users.API.Models;
using Users.API.Services.Email;
using Users.API.Services.Email.Extensions;

namespace Users.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }

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

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<UsersDbContext>()
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddDefaultTokenProviders();

            services.AddDbContext<UsersDbContext>(options =>
            {
                options.UseMySql(Configuration["ConnectionString"],
                    opt =>
                    {
                        opt.MigrationsAssembly(typeof(Startup).Assembly.FullName);
                        opt.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: Array.Empty<int>());
                    });
            });

            services.AddAuthenticationWithBypass(opt =>
            {
                opt.Authority = Configuration["AuthUrl"];
                opt.Audience = "UsersApi";
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

            services.AddEventBus(options =>
            {
                options.HostName = Configuration["EventBusConnection"];
                options.UserName = Configuration["EventBusUserName"];
                options.Password = Configuration["EventBusPassword"];
                options.QueueName = Configuration["EventBusQueueName"];
            }, GetType().Assembly);

            services.AddScoped<UsersDbContextSeed>();

            services.AddHealthChecks()
                .AddCheck("self-check", () => HealthCheckResult.Healthy())
                .AddCheck("db-check", new MySqlConnectionHealthCheck(Configuration["ConnectionString"]));

            services.AddAutoMapper(config => 
                    config.AddProfile(new AutoMapperConfig(new [] {typeof(Startup).Assembly})),
                    typeof(Startup).Assembly);

            services.AddEmailSender(Configuration, HostingEnvironment);

            services.AddTransient<IEmailGenerator, DefaultEmailGenerator>();

            services.AddMediatR(typeof(Startup).Assembly);

            services.AddScoped<IUsersQueries, DefaultUsersQueries>();
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
        }
    }
}
