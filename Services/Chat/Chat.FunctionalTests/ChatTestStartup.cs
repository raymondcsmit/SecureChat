using System.Threading.Tasks;
using Chat.API;
using Chat.API.Infrastructure.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SecureChat.Common.Events.EventBus.Abstractions;
using SecureChat.Common.Events.EventBus.Events;

namespace Chat.FunctionalTests
{
    public class ChatTestStartup : Startup
    {
        public static readonly Mock<IEventBus> EventBusMock = new Mock<IEventBus>();

        public ChatTestStartup(IConfiguration configuration, IHostingEnvironment env) : base(configuration, env)
        {
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
            EventBusMock.Setup(mock => mock.Publish(It.IsAny<IntegrationEvent>()));
            services.AddSingleton(_ => EventBusMock.Object);
        }
    }
}
