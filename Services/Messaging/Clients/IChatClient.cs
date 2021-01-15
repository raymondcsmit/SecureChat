using Messaging.Dtos;
using System.Threading.Tasks;

namespace Messaging.Clients
{
    public interface IChatClient
    {
        Task FriendshipRequestReceived(FriendshipRequestDto friendshipRequest);

        Task FriendshipCreated(FriendshipDto friendship);

        Task ReceiveNotification(object data);
    }
}
