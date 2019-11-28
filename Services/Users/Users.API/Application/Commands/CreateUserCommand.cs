using System.ComponentModel.DataAnnotations;
using Helpers.Mapping;
using MediatR;
using Users.API.Dtos;
using Users.API.Models;

namespace Users.API.Application.Commands
{
    public class CreateUserCommand : IRequest<UserDto>, IMapTo<User>
    {
        public CreateUserCommand(string userName, string email, string password)
        {
            UserName = userName;
            Email = email;
            Password = password;
        }

        [Required]
        public string UserName { get; }

        [Required, EmailAddress]
        public string Email { get; }

        [Required]
        public string Password { get; }
    }
}
