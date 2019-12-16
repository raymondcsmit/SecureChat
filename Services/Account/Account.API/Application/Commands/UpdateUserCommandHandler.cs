using System.Threading;
using System.Threading.Tasks;
using Account.API.Application.IntegrationEvents.Events;
using Account.API.Infrastructure.Exceptions;
using Account.API.Models;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using SecureChat.Common.Events.EventBus.Abstractions;


namespace Account.API.Application.Commands
{
    public class UpdateUserCommandHandler : INotificationHandler<UpdateUserCommand>
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UpdateUserCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IEventBus _eventBus;

        public UpdateUserCommandHandler(
            UserManager<User> userManager,
            ILogger<UpdateUserCommandHandler> logger,
            IMapper mapper,
            IEventBus eventBus)
        {
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
            _eventBus = eventBus;
        }

        public async Task Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(command.Id);
            if (user == null)
            {
                _logger.LogWarning($"Update (invalid id): {command.Id}");
                throw new UsersApiException("User update failed", new[] { "User not found" }, 404);
            }

            var userPatch = _mapper.Map<JsonPatchDocument<User>>(command.Patch);
            userPatch.ApplyTo(user);
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new UsersApiException("User could not be updated", result.Errors);
            }
            _logger.LogInformation($"Successfully updated user with id {command.Id}");

            _eventBus.Publish(new UserAccountUpdatedIntegrationEvent()
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email
            });
        }
    }
}
