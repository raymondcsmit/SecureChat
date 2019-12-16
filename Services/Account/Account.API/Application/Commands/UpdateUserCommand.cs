using Account.API.Dtos;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;

namespace Account.API.Application.Commands
{
    public class UpdateUserCommand : INotification
    {
        public string Id { get; }
        public JsonPatchDocument<UserDto> Patch { get; }

        public UpdateUserCommand(string id, JsonPatchDocument<UserDto> patch)
        {
            Id = id;
            Patch = patch;
        }
    }
}
