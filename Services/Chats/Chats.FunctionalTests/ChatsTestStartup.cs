using System.Threading.Tasks;
using Chats.Api;
using Chats.Api.Dtos;
using Chats.Api.Infrastructure.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Users.API.Client;

namespace Chats.FunctionalTests
{
    public class ChatsTestStartup : Startup
    {
        public ChatsTestStartup(IConfiguration configuration) : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddControllers(options =>
                {
                    options.Filters.Add(typeof(GlobalExceptionFilter));
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                })
                .AddApplicationPart(typeof(Startup).Assembly);

            // Replace UsersApiClient with a mock
            var usersApiClientMock = new Mock<IUsersApiClient>();
            var mockUser = new Users.API.Client.Dtos.UserDto()
            {
                Id = "system",
                UserName = "system_user",
                Email = "system@system"
            };

            usersApiClientMock.Setup(client => client.GetUserById(It.IsAny<string>())).Returns(Task.FromResult(mockUser));
            services.AddTransient(_ => usersApiClientMock.Object);
        }
    }
}
