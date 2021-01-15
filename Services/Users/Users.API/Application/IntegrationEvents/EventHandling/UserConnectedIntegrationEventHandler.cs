using System.Linq;
using System.Threading.Tasks;
using SecureChat.Common.Events.EventBus.Abstractions;
using Users.API.Application.IntegrationEvents.Events;
using Users.API.Application.Queries;

namespace Users.API.Application.IntegrationEvents.EventHandling
{
    public class UserConnectedIntegrationEventHandler : IIntegrationEventHandler<UserConnectedIntegrationEvent>
    {
        private readonly IFriendshipQueries _friendshipQueries;
        private readonly IEventBus _eventBus;

        public UserConnectedIntegrationEventHandler(IFriendshipQueries friendshipQueries, IEventBus eventBus)
        {
            _friendshipQueries = friendshipQueries;
            _eventBus = eventBus;
        }

        public async Task Handle(UserConnectedIntegrationEvent @event, bool redelivered)
        {
            var (friendships, _) = await _friendshipQueries.GetFriendshipsByUserId(@event.UserId);
            var userIds = friendships
                .SelectMany(f => new[] {f.User1.Id, f.User2.Id})
                .Where(id => id != @event.UserId);
            _eventBus.Publish(new NotifyUsersIntegrationEvent(userIds, "UserConnected", new {userId = @event.UserId}));
        }
    }
}
