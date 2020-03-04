using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Chat.API.Dtos;
using Chat.API.Infrastructure.Exceptions;
using Chat.Domain.AggregateModel.UserAggregate;
using Helpers.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;
using Profile = Chat.Domain.AggregateModel.UserAggregate.Profile;

namespace Chat.API.Application.Commands
{
    public class UpdateUserCommandHandler : INotificationHandler<UpdateUserCommand>
    {
        private readonly ILogger<UpdateUserCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

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
            var user = await _userRepository.GetByIdAsync(command.Id);
            if (user == null)
            {
                throw new ChatApiException("User update failed", new[] { "User not found" }, 404);
            }

            await EnsureUniqueUserNameEmail(command);

            command.Patch.ApplyTo(user, _mapper);
            _userRepository.Update(user);
             await _userRepository.UnitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Successfully updated user with id {command.Id}");
        }

        private async Task EnsureUniqueUserNameEmail(UpdateUserCommand command)
        {
            var dto = new UserDto()
            {
                Profile = new ProfileDto()
            };
            command.Patch.ApplyTo(dto);
            var (userNameExists, emailExists) = await _userRepository.UserNameOrEmailExists(dto.UserName, dto.Email);
            if (userNameExists)
            {
                throw new ChatApiException("User Update Failed", new[] { "UserName already in use" }, 400);
            }
            if (emailExists)
            {
                throw new ChatApiException("User Update Failed", new[] { "Email already in use" }, 400);
            }
        }
    }
}
