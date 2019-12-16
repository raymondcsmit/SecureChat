﻿using System.Threading;
using System.Threading.Tasks;
using Account.API.Application.IntegrationEvents.Events;
using Account.API.Dtos;
using Account.API.Infrastructure.Exceptions;
using Account.API.Models;
using Account.API.Services.Email;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SecureChat.Common.Events.EventBus.Abstractions;


namespace Account.API.Application.Commands
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<CreateUserCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IEmailGenerator _emailGenerator;
        private readonly IEmailSender _emailSender;
        private readonly IEventBus _eventBus;

        public CreateUserCommandHandler(
            UserManager<User> userManager,
            ILogger<CreateUserCommandHandler> logger,
            IMapper mapper,
            IEmailGenerator emailGenerator,
            IEmailSender emailSender,
            IEventBus eventBus)
        {
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
            _emailGenerator = emailGenerator;
            _emailSender = emailSender;
            _eventBus = eventBus;
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
            _eventBus.Publish(new UserRegisteredIntegrationEvent()
            {
                UserId = createdUser.Id,
                UserName = createdUser.UserName,
                Email = createdUser.Email
            });
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var (subject, body) =
                _emailGenerator.GenerateEmailConfirmationEmail(user.UserName, token);
            await _emailSender.SendEmailAsync(user.Email, subject, body);
            return _mapper.Map<UserDto>(createdUser);
        }
    }
}