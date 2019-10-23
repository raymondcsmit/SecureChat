using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Users.API.Dtos;
using Users.API.Infrastructure.Exceptions;
using Users.API.Models;
using Users.API.Services.Email;

namespace Users.API.Application.Commands
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<CreateUserCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IEmailGenerator _emailGenerator;
        private readonly IEmailSender _emailSender;

        public CreateUserCommandHandler(
            UserManager<User> userManager,
            ILogger<CreateUserCommandHandler> logger,
            IMapper mapper,
            IEmailGenerator emailGenerator,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
            _emailGenerator = emailGenerator;
            _emailSender = emailSender;
        }

        public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = _mapper.Map<User>(request);
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                throw new UsersApiException("User could not be created", result.Errors);
            }
                
            var createdUser = await _userManager.FindByNameAsync(user.UserName);
            await _userManager.AddToRoleAsync(createdUser, "user");
            _logger.LogInformation($"Successfully created user with id {createdUser.Id}");
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var (subject, body) =
                _emailGenerator.GenerateEmailConfirmationEmail(user.UserName, token);
            await _emailSender.SendEmailAsync(user.Email, subject, body);
            return _mapper.Map<UserDto>(createdUser);
        }
    }
}
