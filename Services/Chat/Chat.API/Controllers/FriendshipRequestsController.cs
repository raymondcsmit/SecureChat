using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.API.Application.Commands;
using Chat.API.Application.Queries;
using Chat.API.Application.Specifications;
using Chat.API.Dtos;
using Chat.API.Models;
using Helpers.Auth;
using Helpers.Auth.AuthHelper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chat.API.Controllers
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
        public async Task<IActionResult> CreateFriendshipRequest([FromBody] CreateFriendshipRequestCommand makeFriendshipRequestCommand)
        {
            var authHelper = new AuthHelperBuilder()
                .AllowSystem()
                .AllowId(makeFriendshipRequestCommand.RequesterId)
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

            var friendshipRequestDto = await _mediator.Send(makeFriendshipRequestCommand);
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