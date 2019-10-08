using System.Threading.Tasks;

namespace SecureChat.Common.Events.EventBus.Abstractions
{
    public interface IDynamicIntegrationEventHandler
    {
        Task Handle(dynamic eventData);
    }
}
