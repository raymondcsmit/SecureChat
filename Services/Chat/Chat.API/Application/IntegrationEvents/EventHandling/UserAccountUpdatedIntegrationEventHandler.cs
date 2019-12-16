using System.Threading.Tasks;
using Chat.API.Application.IntegrationEvents.Events;
using Chat.Domain.AggregateModel.UserAggregate;
using Microsoft.Extensions.Logging;
using SecureChat.Common.Events.EventBus.Abstractions;

namespace Chat.API.Application.IntegrationEvents.EventHandling
{
    public class UserAccountUpdatedIntegrationEventHandler : IIntegrationEventHandler<UserAccountUpdatedIntegrationEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserAccountUpdatedIntegrationEventHandler> _logger;

        public UserAccountUpdatedIntegrationEventHandler(
            IUserRepository userRepository,
            ILogger<UserAccountUpdatedIntegrationEventHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task Handle(UserAccountUpdatedIntegrationEvent @event, bool redelivered)
        {
            var existingUser = await _userRepository.GetAsync(@event.UserId);
            if (existingUser == null)
            {
                _userRepository.Add(new User(@event.UserId, @event.UserName));
            }
            else
            {
                existingUser.UpdateUsername(@event.UserName);
            }

            await _userRepository.UnitOfWork.SaveChangesAsync();
        }
    }
}
