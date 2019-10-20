using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Users.API;
using Users.API.Infrastructure.Filters;
using Users.API.Services.Email;

namespace Users.FunctionalTests
{
    public class UsersTestStartup : Startup
    {
        public static readonly Mock<IEmailSender> EmailSenderMock = new Mock<IEmailSender>();

        public UsersTestStartup(IConfiguration configuration, IHostingEnvironment env) : base(configuration, env)
        {
        }

        public override void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (Configuration["isTest"] == bool.TrueString.ToLowerInvariant())
            {
                app.UseMiddleware<AutoAuthorizeMiddleware>();
            }
            base.Configure(app, env);
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddMvc(options =>
                {
                    options.Filters.Add(typeof(GlobalExceptionFilter));
                })
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                })
                .AddApplicationPart(typeof(Startup).Assembly);

            EmailSenderMock.Setup(mock => mock.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);
            services.AddScoped(_ => EmailSenderMock.Object);
        }
    }
}
