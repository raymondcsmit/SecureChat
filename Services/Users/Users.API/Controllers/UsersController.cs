using System.Threading.Tasks;
using Users.API.Application.Commands;
using Users.API.Application.Queries;
using Users.API.Application.Specifications;
using Users.API.Dtos;
using Users.API.Models;
using Helpers.Auth;
using Helpers.Auth.AuthHelper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Users.API.Controllers
{
    [Route("api/users")]
    [Authorize]
    public class UsersController : Controller
    {
        private readonly IIdentityService _identityService;
        private readonly IMediator _mediator;
        private readonly IUserQueries _userQueries;

        public UsersController(
            IIdentityService identityService,
            IMediator mediator,
            IUserQueries userQueries)
        {
            _identityService = identityService;
            _mediator = mediator;
            _userQueries = userQueries;
        }

        [HttpGet("{id}", Name = nameof(GetUserById))]
        public async Task<IActionResult> GetUserById([FromRoute] string id)
        {
            var authHelper = new AuthHelperBuilder()
                .AllowSystem()
                .AllowId(id)
                .RequirePermissions("users.view_others")
                .Build();

            if (!authHelper.Authorize(_identityService))
            {
                return Unauthorized();
            }

            var user = await _userQueries.GetUserByIdAsync(id);
            return Ok(user);
        }

        [HttpGet("me", Name = nameof(GetMe))]
        public async Task<IActionResult> GetMe()
        {
            var myId = _identityService.GetUserIdentity();

            var authHelper = new AuthHelperBuilder()
                .AllowId(myId)
                .Build();

            if (!authHelper.Authorize(_identityService))
            {
                return Unauthorized();
            }

            var user = await _userQueries.GetUserByIdAsync(myId);
            return Ok(user);
        }

        [HttpPatch("{id}", Name = nameof(UpdateUserById))]
        public async Task<IActionResult> UpdateUserById([FromRoute] string id, [FromBody] JsonPatchDocument<UserDto> patch)
        {
            var authHelper = new AuthHelperBuilder()
                .AllowSystem()
                .AllowId(id)
                .RequirePermissions("users.update_others")
                .Build();

            if (!authHelper.Authorize(_identityService))
            {
                return Unauthorized();
            }

            // TODO - refactor
            var testDto = UserDto.ValidationUser;
            patch.ApplyTo(testDto, ModelState);
            TryValidateModel(testDto);
            //TryValidateModel(testDto.Profile);
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse(ModelState));
            }

            await _mediator.Publish(new UpdateUserCommand(id, patch));
            return NoContent();
        }

        [HttpGet(Name = nameof(GetUsers))]
        public async Task<IActionResult> GetUsers(QueryDto query)
        {
            var authHelper = new AuthHelperBuilder()
                .AllowSystem()
                .RequirePermissions("users.view_others")
                .Build();

            if (!authHelper.Authorize(_identityService))
            {
                return Unauthorized();
            }

            var myId = _identityService.GetUserIdentity();
            var spec = myId == AuthorizationConstants.System ? new UserSpecification(query) : new UserSpecification(query, new[] {myId});

            var (users, total) = await _userQueries.GetUsersAsync(spec);
            return Ok(new ArrayResponse<UserDto>(users, total));
        }
    }
}
