using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SecureChat.Common.Events.EventBus.Abstractions;
using Users.Domain.AggregateModel.UserAggregate;
using Users.API.Infrastructure.Exceptions;
using Users.API.Application.Specifications;
using Users.API.Application.Queries;
using Users.API.Application.IntegrationEvents.Events;

namespace Users.API.Application.Commands
{
    public class DeleteFriendshipByFriendIdCommandHandler: INotificationHandler<DeleteFriendshipByFriendIdCommand>
    {
        private readonly ILogger<DeleteFriendshipByFriendIdCommandHandler> _logger;
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IEventBus _eventBus;
        private readonly IFriendshipQueries _queries;

        public DeleteFriendshipByFriendIdCommandHandler(
            ILogger<DeleteFriendshipByFriendIdCommandHandler> logger,
            IFriendshipRepository friendshipRepository,
            IEventBus eventBus,
            IFriendshipQueries queries)
        {
            _logger = logger;
            _friendshipRepository = friendshipRepository;
            _eventBus = eventBus;
            _queries = queries;
        }

        public async Task Handle(DeleteFriendshipByFriendIdCommand command, CancellationToken cancellationToken)
        {
            if (command.UserId == null || command.FriendId == null)
            {
                throw new ChatApiException("Could not delete friendship", new[] {"UserId or FriendId is null"});
            }

            var spec1 = new FriendshipSpecification(command.UserId, command.FriendId);
            var spec2 = new FriendshipSpecification(command.FriendId, command.UserId);
            var (friendships1, _) = await _queries.GetFriendships(spec1);
            var (friendships2, _) = await _queries.GetFriendships(spec2);
            var friendship = friendships1.FirstOrDefault() ?? friendships2.FirstOrDefault();
            if (friendship == null)
            {
                throw new ChatApiException("Could not delete friendship", new[] { "Friendship not found" }, 404);
            }

            _friendshipRepository.DeleteById(friendship.Id);
            await _friendshipRepository.UnitOfWork.SaveChangesAsync();
            _logger.LogInformation($"Friendship between {command.UserId} and {command.UserId} deleted");
            _eventBus.Publish(new NotifyUsersIntegrationEvent(new [] {command.UserId, command.FriendId}, "FriendshipDeleted", new { friendship }));
        }
    }
}
