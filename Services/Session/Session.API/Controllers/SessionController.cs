using System;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Session.API.Services;

namespace Session.API.Controllers
{
    [Route("api/sessions")]
    [Authorize]
    public class SessionController: ControllerBase
    {
        private readonly ISessionService _sessionService;
        private readonly IIdentityService _identityService;

        public SessionController(
            ISessionService sessionService, IIdentityService identityService)
        {
            _sessionService = sessionService;
            _identityService = identityService;
        }

        [HttpPost(Name = nameof(CreateSessionAsync))]
        public async Task<IActionResult> CreateSessionAsync()
        {
            var userId = _identityService.GetUserIdentity();

            var session = await _sessionService.CreateSessionAsync(userId);
            return new ObjectResult(session) {StatusCode = StatusCodes.Status201Created};
        }

        [HttpDelete(Name = nameof(EndSessionAsync))]
        public async Task<IActionResult> EndSessionAsync()
        {
            var userId = _identityService.GetUserIdentity();

            await _sessionService.EndSessionAsync(userId);
            return Ok();
        }
        
        [HttpPatch(Name = nameof(RefreshSessionAsync))]
        public async Task<IActionResult> RefreshSessionAsync()
        {
            var userId = _identityService.GetUserIdentity();

            await _sessionService.RefreshSessionAsync(userId);
            return Ok();
        }

        [HttpGet(Name = nameof(GetFriendSessions))]
        public async Task<IActionResult> GetFriendSessions()
        {
            var userId = _identityService.GetUserIdentity();

            var sessions = await _sessionService.GetChatSessionsForFriendsById(userId);
            return Ok(sessions);
        }
    }
}
