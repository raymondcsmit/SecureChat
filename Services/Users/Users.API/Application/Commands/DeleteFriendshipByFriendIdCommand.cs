using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Users.API.Application.Commands
{
    public class DeleteFriendshipByFriendIdCommand: INotification
    {
        public string UserId { get; }
        public string FriendId { get; }

        public DeleteFriendshipByFriendIdCommand(string userId, string friendId)
        {
            UserId = userId;
            FriendId = friendId;
        }
    }
}
