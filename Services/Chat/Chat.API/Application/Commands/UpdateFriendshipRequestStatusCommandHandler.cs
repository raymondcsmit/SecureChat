using Chat.API.Infrastructure.Exceptions;
using Chat.Domain.AggregateModel.UserAggregate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.API.Application.Commands
{
    public class UpdateFriendshipRequestStatusCommandHandler : INotificationHandler<UpdateFriendshipRequestStatusCommand>
    {
        private readonly IFriendshipRequestRepository _friendshipRequestRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFriendshipRepository _friendshipRepository;

        public UpdateFriendshipRequestStatusCommandHandler(
            IFriendshipRequestRepository friendshipRequestRepository,
            IUserRepository userRepository,
            IFriendshipRepository friendshipRepository)
        {
            _friendshipRequestRepository = friendshipRequestRepository;
            _userRepository = userRepository;
            _friendshipRepository = friendshipRepository;
        }

        public async Task Handle(UpdateFriendshipRequestStatusCommand command, CancellationToken cancellationToken)
        {
            var friendshipRequest = await _friendshipRequestRepository.GetByIdAsync(command.Id);
            if (friendshipRequest == null)
            {
                throw new ChatApiException("Could not update friendship request status", Enumerable.Empty<string>(), 404);
            }

            var requestee = await _userRepository.GetByIdAsync(friendshipRequest.RequesteeId);

            if (command.Outcome == FriendshipRequest.Outcomes.Accepted)
            {
                requestee.AcceptFriendshipRequest(friendshipRequest.RequesterId);
                _friendshipRepository.Create(requestee.Friendships.First(f => f.User1Id == friendshipRequest.RequesterId));
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
        }
    }
}
