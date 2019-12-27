using System.Linq;
using System.Threading.Tasks;
using Chat.API.Application.Commands;
using Chat.API.Dtos;
using Chat.API.Models;
using Chat.API.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Chat.API.Controllers
{
    [Route("api/users")]
    [Authorize]
    public class UsersController : Controller
    {
        private readonly IIdentityService _identityService;
        private readonly IMediator _mediator;

        public UsersController(
            IIdentityService identityService,
            IMediator mediator)
        {
            _identityService = identityService;
            _mediator = mediator;
        }

        [HttpPatch("{id}", Name = nameof(UpdateUserById))]
        public async Task<IActionResult> UpdateUserById([FromRoute] string id, [FromBody] JsonPatchDocument<UserUpdateDto> patch)
        {
            var testDto = new UserUpdateDto();
            patch.ApplyTo(testDto, ModelState);
            TryValidateModel(testDto);
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse(ModelState));
            }

            var myId = _identityService.GetUserIdentity();
            var myPermissions = _identityService.GetPermissions();
            if (myId != "system" && myId != id && !myPermissions.Contains("users.update"))
            {
                return Unauthorized();
            }

            await _mediator.Publish(new UpdateUserCommand(id, patch));
            return NoContent();
        }
    }
}
