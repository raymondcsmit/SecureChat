using System.Threading.Tasks;
using Chat.API.Application.IntegrationEvents.Events;
using Chat.Domain.AggregateModel.UserAggregate;
using Microsoft.Extensions.Logging;
using SecureChat.Common.Events.EventBus.Abstractions;

namespace Chat.API.Application.IntegrationEvents.EventHandling
{
    public class UserRegisteredIntegrationEventHandler : IIntegrationEventHandler<UserRegisteredIntegrationEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserRegisteredIntegrationEventHandler> _logger;

        public UserRegisteredIntegrationEventHandler(
            IUserRepository userRepository,
            ILogger<UserRegisteredIntegrationEventHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task Handle(UserRegisteredIntegrationEvent @event, bool redelivered)
        {
            var existingUser = await _userRepository.GetAsync(@event.UserId);
            if (existingUser != null)
            {
                _userRepository.Add(new User(@event.UserId, @event.UserName));
                await _userRepository.UnitOfWork.SaveChangesAsync();
            }
        }
    }
}
