using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Chat.API.Application.IntegrationEvents.Events;
using Chat.Domain.Events;
using MediatR;
using SecureChat.Common.Events.EventBus.Abstractions;

namespace Chat.API.Application.DomainEventHandlers
{
    public class UserNameUpdatedDomainEventHandler : INotificationHandler<UserNameUpdatedDomainEvent>
    {
        private readonly IEventBus _eventBus;
        private readonly IMapper _mapper;

        public UserNameUpdatedDomainEventHandler(IEventBus eventBus, IMapper mapper)
        {
            _eventBus = eventBus;
            _mapper = mapper;
        }

        public async Task Handle(UserNameUpdatedDomainEvent notification, CancellationToken cancellationToken)
        {
            _eventBus.Publish(new UserAccountUpdatedIntegrationEvent(notification.Id) {Email = notification.NewUserName});
            await Task.CompletedTask;
        }
    }
}
