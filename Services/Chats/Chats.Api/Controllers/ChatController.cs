using System;
using System.Collections;
using System.Threading.Tasks;
using Chats.Api.Application.Commands;
using Chats.Api.Application.Queries;
using Chats.Api.Application.Specifications;
using Chats.Api.Dtos;
using Helpers.Auth;
using Helpers.Auth.AuthHelper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chats.Api.Controllers
{
    [Route("api/chats")]
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IIdentityService _identityService;
        private readonly IMediator _mediator;
        private readonly IChatQueries _chatQueries;

        public ChatController(
            IIdentityService identityService,
            IMediator mediator,
            IChatQueries chatQueries)
        {
            _identityService = identityService;
            _mediator = mediator;
            _chatQueries = chatQueries;
        }

        [HttpPost("", Name = nameof(CreateChat))]
        public async Task<IActionResult> CreateChat([FromBody] CreateChatCommand createChatCommand)
        {
            var authHelper = new AuthHelperBuilder()
                .AllowSystem()
                .RequireId(createChatCommand.OwnerId)
                .RequirePermissions("chats.create")
                .Build();

            if (!authHelper.Authorize(_identityService))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse(ModelState));
            }

            var chatDto = await _mediator.Send(createChatCommand);
            var url = Url.Action(nameof(GetChatById), new {id = chatDto.Id});
            return Created(url, chatDto);
        }

        [HttpGet("{id}", Name = nameof(GetChatById))]
        public async Task<IActionResult> GetChatById(string id)
        {
            var authHelper = new AuthHelperBuilder()
                .AllowSystem()
                .RequirePermissions("chats.manage")
                .Build();

            if (!authHelper.Authorize(_identityService))
            {
                return Unauthorized();
            }

            var chat = await _chatQueries.GetChatById(id);
            if (chat == null)
            {
                return NotFound();
            }
            return Ok(chat);
        }

        [HttpGet("", Name = nameof(GetChats))]
        public async Task<IActionResult> GetChats(QueryDto query)
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

            var userId = _identityService.GetUserIdentity();

            var (chats, total) = userId == AuthorizationConstants.System ?
                await _chatQueries.GetChats(new ChatSpecification(query)) :
                await _chatQueries.GetChatsForOwnerOrMemberAsync(new ChatSpecification(query), userId);
            return Ok(new ArrayResponse<ChatDto>(chats, total));
        }
    }
}