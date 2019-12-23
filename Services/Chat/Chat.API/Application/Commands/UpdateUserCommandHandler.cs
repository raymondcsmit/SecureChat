using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Chat.API.Infrastructure.Exceptions;
using Chat.Domain.AggregateModel.UserAggregate;
using Helpers.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;
using SecureChat.Common.Events.EventBus.Abstractions;

namespace Chat.API.Application.Commands
{
    public class UpdateUserCommandHandler : INotificationHandler<UpdateUserCommand>
    {
        private readonly ILogger<UpdateUserCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IEventBus _eventBus;

        public UpdateUserCommandHandler(
            ILogger<UpdateUserCommandHandler> logger,
            IMapper mapper,
            IUserRepository userRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetAsync(command.Id);
            if (user == null)
            {
                throw new ChatApplicationException("User update failed", new[] { "User not found" }, 404);
            }

            command.Patch.ApplyTo(user, _mapper);
            command.Patch.ApplyTo(user.Profile, _mapper);
            _userRepository.Update(user);
            _logger.LogInformation($"Successfully updated user with id {command.Id}");
        }
    }
}
