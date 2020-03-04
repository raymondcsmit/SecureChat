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
            if (await _userRepository.GetByIdAsync(@event.UserId) != null)
            {
                _logger.LogWarning($"User account with id {@event.UserId} already exists. No action taken.");
            }
            _userRepository.Create(new User(@event.UserId, @event.UserName, @event.Email));
            await _userRepository.UnitOfWork.SaveChangesAsync();
        }
    }
}
