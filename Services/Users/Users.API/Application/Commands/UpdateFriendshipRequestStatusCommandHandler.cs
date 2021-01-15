using Users.API.Infrastructure.Exceptions;
using Users.Domain.AggregateModel.UserAggregate;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SecureChat.Common.Events.EventBus.Abstractions;
using Users.API.Application.IntegrationEvents.Events;
using Users.API.Application.Queries;

namespace Users.API.Application.Commands
{
    public class UpdateFriendshipRequestStatusCommandHandler : INotificationHandler<UpdateFriendshipRequestStatusCommand>
    {
        private readonly IFriendshipRequestRepository _friendshipRequestRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IEventBus _eventBus;
        private readonly IFriendshipQueries _friendshipQueries;

        public UpdateFriendshipRequestStatusCommandHandler(
            IFriendshipRequestRepository friendshipRequestRepository,
            IUserRepository userRepository,
            IFriendshipRepository friendshipRepository,
            IEventBus eventBus,
            IFriendshipQueries friendshipQueries)
        {
            _friendshipRequestRepository = friendshipRequestRepository;
            _userRepository = userRepository;
            _friendshipRepository = friendshipRepository;
            _eventBus = eventBus;
            _friendshipQueries = friendshipQueries;
        }

        public async Task Handle(UpdateFriendshipRequestStatusCommand command, CancellationToken cancellationToken)
        {
            var friendshipRequest = await _friendshipRequestRepository.GetByIdAsync(command.Id);
            if (friendshipRequest == null)
            {
                throw new ChatApiException("Could not update friendship request status", Enumerable.Empty<string>(), 404);
            }

            var requestee = await _userRepository.GetByIdAsync(friendshipRequest.RequesteeId);

            Friendship friendship = null;
            if (command.Outcome == FriendshipRequest.Outcomes.Accepted)
            {
                requestee.AcceptFriendshipRequest(friendshipRequest.RequesterId);
                friendship = requestee.Friendships.First(f => f.User1Id == friendshipRequest.RequesterId);
                _friendshipRepository.Create(friendship);
            }
            else if (command.Outcome == FriendshipRequest.Outcomes.Rejected)
            {
                requestee.RejectFriendshipRequest(friendshipRequest.RequesterId);
            }
            else
            {
                throw new ChatApiException("Invalid friendship request outcome", new[] { $"{command.Outcome} is invalid" });
            }

            _friendshipRequestRepository.Update(requestee.FriendshipRequests.First(req => req.Id == command.Id));
            await _friendshipRequestRepository.UnitOfWork.SaveChangesAsync();

            if (command.Outcome == FriendshipRequest.Outcomes.Accepted)
            {
                var dto = await _friendshipQueries.GetFriendshipById(friendship.Id);
                _eventBus.Publish(new FriendshipCreatedIntegrationEvent(dto));
            }
        }
    }
}
