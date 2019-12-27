using System.Threading.Tasks;
using Chat.API.Application.IntegrationEvents.Events;
using Chat.Domain.AggregateModel.UserAggregate;
using Microsoft.Extensions.Logging;
using SecureChat.Common.Events.EventBus.Abstractions;

namespace Chat.API.Application.IntegrationEvents.EventHandling
{
    public class UserAccountCreatedIntegrationEventHandler : IIntegrationEventHandler<UserAccountCreatedIntegrationEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserAccountCreatedIntegrationEventHandler> _logger;

        public UserAccountCreatedIntegrationEventHandler(
            IUserRepository userRepository,
            ILogger<UserAccountCreatedIntegrationEventHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task Handle(UserAccountCreatedIntegrationEvent @event, bool redelivered)
        {
            var existingUser = await _userRepository.GetAsync(@event.UserId);
            if (existingUser != null)
            {
                _userRepository.Add(new User(@event.UserId, @event.UserName, @event.Email));
                await _userRepository.UnitOfWork.SaveChangesAsync();
            }
        }
    }
}
