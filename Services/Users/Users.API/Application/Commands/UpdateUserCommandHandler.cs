using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Users.API.Infrastructure.Exceptions;
using Users.API.Models;

namespace Users.API.Application.Commands
{
    public class UpdateUserCommandHandler : INotificationHandler<UpdateUserCommand>
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UpdateUserCommandHandler> _logger;
        private readonly IMapper _mapper;

        public UpdateUserCommandHandler(
            UserManager<User> userManager,
            ILogger<UpdateUserCommandHandler> logger,
            IMapper mapper)
        {
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(command.Id);
            if (user == null)
            {
                _logger.LogWarning($"Update (invalid id): {command.Id}");
                throw new UsersApiException("User update failed", new[] { "User not found" }, 404);
            }

            _mapper.Map(command.Patch, user);
            _logger.LogInformation($"Successfully updated user with id {command.Id}");
        }
    }
}
