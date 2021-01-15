using System;
using SecureChat.Common.Events.EventBus;

namespace Chats.FunctionalTests
{
    public class TestEventBusFixture : IDisposable
    {
        public TestEventBus EventBus { get; }

        public TestEventBusFixture()
        {
            EventBus = new TestEventBus(new EventBusOptions()
            {
                HostName = "localhost",
                UserName = "rabbitmq",
                Password = "12345",
                QueueName = "TestDispatcher",
                RetryCount = 5
            });
        }
        public void Dispose()
        {
            EventBus?.Dispose();
        }
    }
}
