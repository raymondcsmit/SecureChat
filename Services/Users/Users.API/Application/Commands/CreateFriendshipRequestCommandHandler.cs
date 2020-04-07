using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Users.API.Application.Queries;
using Users.API.Dtos;
using Users.API.Infrastructure.Exceptions;
using Users.Domain.AggregateModel.UserAggregate;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Users.API.Application.Commands
{
    public class CreateFriendshipRequestCommandHandler : IRequestHandler<CreateFriendshipRequestCommand, FriendshipRequestDto>
    {
        private readonly ILogger<UpdateUserCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IFriendshipRequestRepository _friendshipRequestRepository;
        private readonly IFriendshipRequestQueries _friendshipRequestQueries;

        public CreateFriendshipRequestCommandHandler(
            ILogger<UpdateUserCommandHandler> logger,
            IMapper mapper,
            IUserRepository userRepository,
            IFriendshipRequestRepository friendshipRequestRepository,
            IFriendshipRequestQueries friendshipRequestQueries)
        {
            _logger = logger;
            _mapper = mapper;
            _userRepository = userRepository;
            _friendshipRequestRepository = friendshipRequestRepository;
            _friendshipRequestQueries = friendshipRequestQueries;
        }

        public async Task<FriendshipRequestDto> Handle(CreateFriendshipRequestCommand command, CancellationToken cancellationToken)
        {
            var requester = await _userRepository.GetByIdAsync(command.RequesterId);
            var requestee = await _userRepository.GetByIdAsync(command.RequesteeId);
            if (requester == null || requestee == null)
            {
                throw new ChatApiException("Could not create friendship request", new[] { "User(s) not found" });
            }

            requester.MakeFriendshipRequest(requestee);
            var request = requester.FriendshipRequests.First(req => req.RequesteeId == requestee.Id);
            _friendshipRequestRepository.Create(request);
            await _userRepository.UnitOfWork.SaveChangesAsync();

            _logger.LogInformation($"User {requester.Id} successfully created friendship request with {requestee.Id}");

            return await _friendshipRequestQueries.GetFriendshipRequestById(request.Id);
        }
    }
}
