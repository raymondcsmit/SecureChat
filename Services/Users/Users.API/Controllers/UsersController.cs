using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Users.API.Application.Commands;
using Users.API.Application.Queries;
using Users.API.Infrastructure.Attributes;
using Users.API.Infrastructure.Services;
using Users.API.Models;

namespace Users.API.Controllers
{
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUsersQueries _usersQueries;
        private readonly IIdentityService _identityService;

        public UsersController(
            IMediator mediator,
            IUsersQueries usersQueries,
            IIdentityService identityService)
        {
            _mediator = mediator;
            _usersQueries = usersQueries;
            _identityService = identityService;
        }

        [AllowAnonymous]
        [Throttle(MilliSeconds = 10000)]
        [HttpPost("", Name = nameof(CreateUserAsync))]
        public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserCommand createUserCommand)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse(ModelState));
            }

            var user = await _mediator.Send(createUserCommand);
            return Created(Url.Action(nameof(GetUserByIdAsync), new {id = user.Id}), user);
        }

        [HttpGet("me", Name = nameof(GetAuthenticatedUser))]
        public async Task<IActionResult> GetAuthenticatedUser()
        {
            var id = _identityService.GetUserIdentity();

            var user = await _usersQueries.GetUserByIdAsync(id);
            return Ok(user);
        }

        [HttpGet("{id}", Name = nameof(GetUserByIdAsync))]
        public async Task<IActionResult> GetUserByIdAsync([FromRoute] string id)
        {
            var user = await _usersQueries.GetUserByIdAsync(id);
            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("{userName}/reset-password", Name = nameof(ResetPasswordAsync))]
        public async Task<IActionResult> ResetPasswordAsync([FromRoute] string userName, [FromBody] ResetPasswordCommand resetPasswordCommand)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse(ModelState));
            }

            resetPasswordCommand.UserName = userName;
            await _mediator.Publish(resetPasswordCommand);

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("{id}/complete-password-reset", Name = nameof(CompletePasswordResetAsync))]
        public async Task<IActionResult> CompletePasswordResetAsync([FromRoute] string id, [FromBody] CompletePasswordResetCommand completePasswordResetCommand)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse(ModelState));
            }

            completePasswordResetCommand.Id = id;
            await _mediator.Publish(completePasswordResetCommand);

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("{id}/confirm-email", Name = nameof(ConfirmEmailAsync))]
        public async Task<IActionResult> ConfirmEmailAsync([FromRoute] string id, [FromBody] ConfirmEmailCommand confirmEmailCommand)
        {
            confirmEmailCommand.Id = id;
            await _mediator.Publish(confirmEmailCommand);

            return Ok();
        }

        [HttpPut("{id}", Name = nameof(UpdateUserById))]
        public async Task<IActionResult> UpdateUserById([FromRoute] string id, [FromBody] UpdateUserCommand updateUserCommand)
        {
            var myId = _identityService.GetUserIdentity();
            var myPermissions = _identityService.GetPermissions();
            if (myId != id && !myPermissions.Contains("users.update"))
            {
                return Unauthorized();
            }

            await _mediator.Publish(updateUserCommand);
            return Ok();
        }
    }
}