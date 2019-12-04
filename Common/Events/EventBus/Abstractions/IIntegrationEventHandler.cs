using System.Threading.Tasks;
using SecureChat.Common.Events.EventBus.Events;

namespace SecureChat.Common.Events.EventBus.Abstractions
{
    public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler 
        where TIntegrationEvent: IntegrationEvent
    {
        Task Handle(TIntegrationEvent integrationEvent, bool redelivered);
    }

    public interface IIntegrationEventHandler
    {
    }
}
