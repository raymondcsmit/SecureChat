using System;
using Newtonsoft.Json;

namespace SecureChat.Common.Events.EventBus.Events
{
    public class IntegrationEvent
    {
        public IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTimeOffset.UtcNow;
        }

        [JsonConstructor]
        public IntegrationEvent(Guid id, DateTimeOffset createDate)
        {
            Id = id;
            CreationDate = createDate;
        }

        [JsonProperty]
        public Guid Id { get; private set; }

        [JsonProperty]
        public DateTimeOffset CreationDate { get; private set; }
    }
}
