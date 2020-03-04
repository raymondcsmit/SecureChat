using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chat.Domain.AggregateModel.UserAggregate;
using Chat.Domain.Events;
using MediatR;

namespace Chat.API.Application.DomainEventHandlers
{
    public class FriendshipRequestAcceptedDomainEventHandler : INotificationHandler<FriendshipRequestAcceptedDomainEvent>
    {
        private readonly IFriendshipRequestRepository _friendshipRequestRepository;
        private readonly IFriendshipRepository _friendshipRepository;

        public FriendshipRequestAcceptedDomainEventHandler(
                IFriendshipRequestRepository friendshipRequestRepository,
                IFriendshipRepository friendshipRepository)
        {
            _friendshipRequestRepository = friendshipRequestRepository;
            _friendshipRepository = friendshipRepository;
        }

        public async Task Handle(FriendshipRequestAcceptedDomainEvent e, CancellationToken cancellationToken)
        {
            if (e.FriendshipRequest.Outcome != FriendshipRequest.Outcomes.Accepted)
            {
                throw new InvalidOperationException("The friendship request has not been accepted");
            }

            _friendshipRequestRepository.Update(e.FriendshipRequest);
            _friendshipRepository.Create(new Friendship(e.FriendshipRequest.RequesteeId,
                e.FriendshipRequest.RequesterId));
            await _friendshipRequestRepository.UnitOfWork.SaveChangesAsync();
        }
    }
}
