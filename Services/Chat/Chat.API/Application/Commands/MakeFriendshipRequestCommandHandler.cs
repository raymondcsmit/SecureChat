using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Chat.API.Dtos;
using Chat.API.Infrastructure.Exceptions;
using Chat.Domain.AggregateModel.UserAggregate;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Chat.API.Application.Commands
{
    public class MakeFriendshipRequestCommandHandler : IRequestHandler<MakeFriendshipRequestCommand, FriendshipRequestDto>
    {
        private readonly ILogger<UpdateUserCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public MakeFriendshipRequestCommandHandler(
            ILogger<UpdateUserCommandHandler> logger,
            IMapper mapper,
            IUserRepository userRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<FriendshipRequestDto> Handle(MakeFriendshipRequestCommand command, CancellationToken cancellationToken)
        {
            var requester = await _userRepository.GetAsync(command.RequesterId);
            var requestee = await _userRepository.GetAsync(command.RequesteeId);
            if (requester == null || requestee == null)
            {
                throw new ChatApiException("Friendship request failed", new[] { $"User {requester ?? requestee} not found" }, 404);
            }

            requester.MakeFriendshipRequest(requestee);
            var request = requester.MyPendingFriendshipRequests.First(req => req.RequesteeId == requestee.Id);
            _userRepository.CreateFriendshipRequest(request);
            await _userRepository.UnitOfWork.SaveChangesAsync();

            _logger.LogInformation($"User {requester.Id} successfully made friendship request with {requestee.Id}");

            return new FriendshipRequestDto
            {
                Id = request.Id,
                Requester = _mapper.Map<UserDto>(requester),
                Requestee = _mapper.Map<UserDto>(requestee),
                CreatedAt = request.CreatedAt
            };
        }
    }
}
