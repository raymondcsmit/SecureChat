using Messaging.Clients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Messaging.Hubs
{
    [Authorize]
    public class MessagingHub: Hub<IChatClient>
    {
        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, Context.UserIdentifier);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.UserIdentifier);
            await base.OnDisconnectedAsync(ex);
        }
    }
}
