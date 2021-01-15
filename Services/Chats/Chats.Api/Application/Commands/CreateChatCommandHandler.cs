using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Chats.Api.Dtos;
using Chats.Api.Infrastructure.Exceptions;
using Chats.Domain.AggregateModel;
using Chats.Infrastructure;
using MediatR;
using Microsoft.Extensions.Logging;
using SecureChat.Common.Events.EventBus.Abstractions;
using Users.API.Client;

namespace Chats.Api.Application.Commands
{
    public class CreateChatCommandHandler : IRequestHandler<CreateChatCommand, ChatDto>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IEventBus _eventBus;
        private readonly ILogger<CreateChatCommandHandler> _logger;
        private readonly IUsersApiClient _usersApiClient;
        private readonly IMapper _mapper;
        private readonly ChatsContext _context;

        public CreateChatCommandHandler(
            IChatRepository chatRepository,
            IEventBus eventBus,
            ILogger<CreateChatCommandHandler> logger,
            IUsersApiClient usersApiClient,
            IMapper mapper,
            ChatsContext context)
        {
            _chatRepository = chatRepository;
            _eventBus = eventBus;
            _logger = logger;
            _usersApiClient = usersApiClient;
            _mapper = mapper;
            _context = context;
        }

        public async Task<ChatDto> Handle(CreateChatCommand request, CancellationToken cancellationToken)
        {
            var chatExists = _context.Chats
                .AsQueryable()
                .Any(chat => chat.Name == request.Name && chat.OwnerId == request.OwnerId);
            if (chatExists)
            {
                throw new ChatApiException("Could not create chat",
                    new[] {$"Chat with name {request.Name} already exists for user {request.OwnerId}"});
            }

            var owner = await _usersApiClient.GetUserById(request.OwnerId);
            var chatEntity = new Chat(request.Name, new User(owner.Id, owner.UserName), request.Capacity);
            _chatRepository.Create(chatEntity);
            await _chatRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<ChatDto>(chatEntity);
        }

    }
}
