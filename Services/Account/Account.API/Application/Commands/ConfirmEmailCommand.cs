using MediatR;

namespace Account.API.Application.Commands
{
    public class ConfirmEmailCommand : INotification
    {
        public string Id { get; set; }

        public string Token { get; set; }
    }
}
