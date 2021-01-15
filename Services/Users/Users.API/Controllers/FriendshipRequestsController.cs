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
using Microsoft.AspNetCore.Mvc;

namespace Users.API.Controllers
{
    [Route("api")]
    [Authorize]
    public class FriendshipRequestsController : Controller
    {
        private readonly IIdentityService _identityService;
        private readonly IMediator _mediator;
        private readonly IFriendshipRequestQueries _friendshipRequestQueries;

        public FriendshipRequestsController(
            IIdentityService identityService,
            IMediator mediator,
            IFriendshipRequestQueries friendshipRequestQueries)
        {
            _identityService = identityService;
            _mediator = mediator;
            _friendshipRequestQueries = friendshipRequestQueries;
        }

        [HttpPost("friendship-requests", Name = nameof(CreateFriendshipRequest))]
        public async Task<IActionResult> CreateFriendshipRequest([FromBody] CreateFriendshipRequestCommand createFriendshipRequestCommand)
        {
            var authHelper = new AuthHelperBuilder()
                .AllowSystem()
                .AllowId(createFriendshipRequestCommand.RequesterId)
                .RequirePermissions("users.view_others")
                .Build();

            if (!authHelper.Authorize(_identityService))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse(ModelState));
            }

            var friendshipRequestDto = await _mediator.Send(createFriendshipRequestCommand);
            var url = Url.Action(nameof(GetFriendshipRequestById), new { id = friendshipRequestDto.Id });
            return Created(url, friendshipRequestDto);
        }

        [HttpPost("users/{requesterId}/friendship-requests", Name = nameof(CreateFriendshipRequestsByRequesterId))]
        public async Task<IActionResult> CreateFriendshipRequestsByRequesterId(string requesterId, [FromForm] string requesteeId)
        {
            var authHelper = new AuthHelperBuilder()
                .AllowSystem()
                .RequirePermissions("users.view_others")
                .Build();

            if (!authHelper.Authorize(_identityService))
            {
                return Unauthorized();
            }

            var friendshipRequestDto = await _mediator.Send(new CreateFriendshipRequestCommand(requesterId, requesteeId));
            var url = Url.Action(nameof(GetFriendshipRequestById), new { id = friendshipRequestDto.Id });
            return Created(url, friendshipRequestDto);
        }

        [HttpGet("friendship-requests/{id}", Name = nameof(GetFriendshipRequestById))]
        public async Task<IActionResult> GetFriendshipRequestById(string id)
        {
            var authHelper = new AuthHelperBuilder()
                .AllowSystem()
                .RequirePermissions("users.manage")
                .Build();

            if (!authHelper.Authorize(_identityService))
            {
                return Unauthorized();
            }

            var friendshipRequest = await _friendshipRequestQueries.GetFriendshipRequestById(id);
            if (friendshipRequest == default)
            {
                return NotFound();
            }
            return Ok(friendshipRequest);
        }

        [HttpGet("friendship-requests", Name = nameof(GetFriendshipRequests))]
        public async Task<IActionResult> GetFriendshipRequests(QueryDto query)
        {
            var authHelper = new AuthHelperBuilder()
                .AllowSystem()
                .RequirePermissions("users.manage")
                .Build();

            if (!authHelper.Authorize(_identityService))
            {
                return Unauthorized();
            }

            var spec = new FriendshipRequestSpecification(query);
            var (friendshipRequests, count) = await _friendshipRequestQueries.GetFriendshipRequests(spec);
            return Ok(new ArrayResponse<FriendshipRequestDto>(friendshipRequests, count));
        }

        [HttpGet("users/{requesteeId}/friendship-requests", Name = nameof(GetFriendshipRequestsByRequesteeId))]
        public async Task<IActionResult> GetFriendshipRequestsByRequesteeId(string requesteeId, QueryDto queryDto)
        {
            var authHelper = new AuthHelperBuilder()
                .AllowSystem()
                .RequirePermissions("users.view_others")
                .Build();

            if (!authHelper.Authorize(_identityService))
            {
                return Unauthorized();
            }

            var spec = new FriendshipRequestSpecification(queryDto, requesteeId);
            var (friendshipRequests, count) = await _friendshipRequestQueries.GetFriendshipRequests(spec);
            return Ok(new ArrayResponse<FriendshipRequestDto>(friendshipRequests, count));
        }

        [HttpPatch("friendship-requests/{id}", Name = nameof(UpdateFriendshipRequestStatusById))]
        public async Task<IActionResult> UpdateFriendshipRequestStatusById([FromRoute] string id, [FromBody] UpdateFriendshipRequestStatusCommand command)
        {
            var authHelper = new AuthHelperBuilder()
                .AllowSystem()
                .RequirePermissions("users.view_others")
                .Build();

            if (!authHelper.Authorize(_identityService))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse(ModelState));
            }

            command.Id = id;
            await _mediator.Publish(command);
            return NoContent();
        }
    }
}