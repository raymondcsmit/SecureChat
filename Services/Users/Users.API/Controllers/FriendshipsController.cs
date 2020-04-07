using Users.API.Application.Queries;
using Users.API.Application.Specifications;
using Users.API.Dtos;
using Users.API.Models;
using Helpers.Auth;
using Helpers.Auth.AuthHelper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Users.API.Controllers
{
    [Route("api")]
    [Authorize]
    public class FriendshipsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IIdentityService _identityService;
        private readonly IFriendshipQueries _friendshipQueries;

        public FriendshipsController(
            IMediator mediator,
            IIdentityService identityService,
            IFriendshipQueries friendshipQueries)
        {
            _mediator = mediator;
            _identityService = identityService;
            _friendshipQueries = friendshipQueries;
        }

        [HttpGet("friendships", Name = nameof(GetFriendships))]
        public async Task<IActionResult> GetFriendships(QueryDto query)
        {
            var authHelper = new AuthHelperBuilder()
                .AllowSystem()
                .RequirePermissions("users.manage")
                .Build();

            if (!authHelper.Authorize(_identityService))
            {
                return Unauthorized();
            }

            var spec = new FriendshipSpecification(query);
            var (friendships, count) = await _friendshipQueries.GetFriendships(spec);
            return Ok(new ArrayResponse<FriendshipDto>(friendships, count));
        }

        [HttpGet("users/{userId}/friendships", Name = nameof(GetFriendshipsByUserId))]
        public async Task<IActionResult> GetFriendshipsByUserId(string userId, QueryDto queryDto)
        {
            var authHelper = new AuthHelperBuilder()
                .AllowSystem()
                .RequirePermissions("users.view_others")
                .Build();

            if (!authHelper.Authorize(_identityService))
            {
                return Unauthorized();
            }

            var (friendships, count) = await _friendshipQueries.GetFriendshipsByUserId(userId, queryDto);
            return Ok(new ArrayResponse<FriendshipDto>(friendships, count));
        }
    }
}
