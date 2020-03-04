using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.API.Application.Commands;
using Chat.API.Application.Queries;
using Chat.API.Application.Specifications;
using Chat.API.Dtos;
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
        public async Task<IActionResult> CreateFriendshipRequest(MakeFriendshipRequestCommand makeFriendshipRequestCommand)
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

            var friendshipRequestOutput = await _mediator.Send(makeFriendshipRequestCommand);
            var url = Url.Action(nameof(GetFriendshipRequestsByRequesterId),new { id = makeFriendshipRequestCommand.RequesterId });
            return Created(url, friendshipRequestOutput);
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
            var friendshipRequest = await _friendshipRequestQueries.GetFriendshipRequests(spec);
            return Ok(friendshipRequest);
        }

        [HttpGet("users/{requesterId}/friendship-requests", Name = nameof(GetFriendshipRequestsByRequesterId))]
        public async Task<IActionResult> GetFriendshipRequestsByRequesterId(string requesterId, QueryDto queryDto)
        {
            var authHelper = new AuthHelperBuilder()
                .AllowSystem()
                .RequirePermissions("users.view_others")
                .Build();

            if (!authHelper.Authorize(_identityService))
            {
                return Unauthorized();
            }

            var spec = new FriendshipRequestSpecification(queryDto, requesterId);
            var friendshipRequests = await _friendshipRequestQueries.GetFriendshipRequests(spec);
            return Ok(friendshipRequests);
        }

        [HttpGet("users/{requesterId}/friendship-requests/{requesteeId}", Name = nameof(GetFriendshipRequestByRequesterAndRequesteeIds))]
        public async Task<IActionResult> GetFriendshipRequestByRequesterAndRequesteeIds(string requesterId, string requesteeId)
        {
            var authHelper = new AuthHelperBuilder()
                .AllowSystem()
                .RequirePermissions("users.view_others")
                .Build();

            if (!authHelper.Authorize(_identityService))
            {
                return Unauthorized();
            }

            var spec = new FriendshipRequestSpecification(requesterId, requesteeId);
            var friendshipRequest = await _friendshipRequestQueries.GetFriendshipRequests(spec);
            return Ok(friendshipRequest);
        }
    }
}