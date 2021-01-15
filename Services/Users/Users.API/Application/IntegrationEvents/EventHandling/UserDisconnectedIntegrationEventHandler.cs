using System.Linq;
using System.Threading.Tasks;
using SecureChat.Common.Events.EventBus.Abstractions;
using Users.API.Application.IntegrationEvents.Events;
using Users.API.Application.Queries;

namespace Users.API.Application.IntegrationEvents.EventHandling
{
    public class UserDisconnectedIntegrationEventHandler: IIntegrationEventHandler<UserDisconnectedIntegrationEvent>
    {
        private readonly IFriendshipQueries _friendshipQueries;
        private readonly IEventBus _eventBus;

        public UserDisconnectedIntegrationEventHandler(IFriendshipQueries friendshipQueries, IEventBus eventBus)
        {
            _friendshipQueries = friendshipQueries;
            _eventBus = eventBus;
        }

        public async Task Handle(UserDisconnectedIntegrationEvent @event, bool redelivered)
        {
            var (friendships, _) = await _friendshipQueries.GetFriendshipsByUserId(@event.UserId);
            var userIds = friendships
                .SelectMany(f => new[] { f.User1.Id, f.User2.Id })
                .Where(id => id != @event.UserId);
            _eventBus.Publish(new NotifyUsersIntegrationEvent(userIds, "UserDisconnected", new {userId = @event.UserId}));

        }
    }
}
