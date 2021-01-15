using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SecureChat.Common.Events.EventBus;
using SecureChat.Common.Events.EventBus.Events;
using SecureChat.Common.Events.EventBusRabbitMQ;

namespace Chats.FunctionalTests
{
    public class TestEventBus : IDisposable
    {
        private readonly EventBusOptions _options;
        private readonly DefaultRabbitMQPersistentConnection _persistentConnection;
        private IModel _consumerChannel;
        private const string BrokerName = "event_bus";

        private Dictionary<string, Action<string, string>> _callbacks = new Dictionary<string, Action<string, string>>();

        public TestEventBus(EventBusOptions options)
        {
            _options = options;
            var eventBusOptions = new OptionsWrapper<EventBusOptions>(options);
            var logger = new LoggerFactory().CreateLogger<DefaultRabbitMQPersistentConnection>();
            _persistentConnection = new DefaultRabbitMQPersistentConnection(logger, eventBusOptions);
        }

        public void Publish(IntegrationEvent @event)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            using (var channel = _persistentConnection.CreateModel())
            {
                var eventName = @event.GetType()
                    .Name;

                channel.ExchangeDeclare(exchange: BrokerName,
                    type: "direct");

                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);

                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2; // persistent

                channel.BasicPublish(exchange: BrokerName,
                    routingKey: eventName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);
            }
        }

        public void Subscribe(string eventName, Action<string, string> callback)
        {
            if (_consumerChannel == null)
            {
                _consumerChannel = CreateConsumerChannel();
            }

            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            _callbacks[eventName] = callback;

            using (var channel = _persistentConnection.CreateModel())
            {
                channel.QueueBind(queue: _options.QueueName,
                    exchange: BrokerName,
                    routingKey: eventName);
            }
        }

        private IModel CreateConsumerChannel()
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var channel = _persistentConnection.CreateModel();

            channel.ExchangeDeclare(exchange: BrokerName,
                type: "direct");

            channel.QueueDeclare(queue: _options.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);


            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var eventName = ea.RoutingKey;
                var message = Encoding.UTF8.GetString(ea.Body);

                _callbacks.GetValueOrDefault(eventName)?.Invoke(eventName, message);

                channel.BasicAck(ea.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(queue: _options.QueueName,
                autoAck: false,
                consumer: consumer);

            return channel;
        }

        public void ClearCallbacks() => _callbacks.Clear();

        public void Dispose()
        {
            _callbacks.Clear();
            _consumerChannel?.Dispose();
            _persistentConnection?.Dispose();
        }
    }
}
