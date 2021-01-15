using Messaging.Clients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SecureChat.Common.Events.EventBus.Abstractions;
using System;
using System.Threading.Tasks;
using Messaging.IntegrationEvents.Events;

namespace Messaging.Hubs
{
    [Authorize]
    public class MessagingHub: Hub<IChatClient>
    {
        private readonly IEventBus _eventBus;

        public MessagingHub(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, Context.UserIdentifier);
            _eventBus.Publish(new UserConnectedIntegrationEvent(userId: Context.UserIdentifier));
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.UserIdentifier);
            _eventBus.Publish(new UserDisconnectedIntegrationEvent(userId: Context.UserIdentifier));
            await base.OnDisconnectedAsync(ex);
        }
    }
}
