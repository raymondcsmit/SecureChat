using System.Linq;
using System.Threading.Tasks;
using Account.API.Application.Commands;
using Account.API.Application.Queries;
using Account.API.Dtos;
using Account.API.Infrastructure.Attributes;
using Account.API.Models;
using Account.API.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Account.API.Controllers
{
    [Route("api/users")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserQueries _userQueries;
        private readonly IIdentityService _identityService;

        public UserController(
            IMediator mediator,
            IUserQueries userQueries,
            IIdentityService identityService)
        {
            _mediator = mediator;
            _userQueries = userQueries;
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

            var user = await _userQueries.GetUserByIdAsync(id);
            return Ok(user);
        }

        [HttpGet("{id}", Name = nameof(GetUserByIdAsync))]
        public async Task<IActionResult> GetUserByIdAsync([FromRoute] string id)
        {
            var user = await _userQueries.GetUserByIdAsync(id);
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
    }
}