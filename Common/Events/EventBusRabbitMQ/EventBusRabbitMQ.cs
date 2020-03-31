using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using SecureChat.Common.Events.EventBus;
using SecureChat.Common.Events.EventBus.Abstractions;
using SecureChat.Common.Events.EventBus.Events;

namespace SecureChat.Common.Events.EventBusRabbitMQ
{
    public class EventBusRabbitMQ : IEventBus, IDisposable
    {
        private const string BrokerName = "event_bus";

        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly ILogger<EventBusRabbitMQ> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly EventBusOptions _options;
        private readonly IEventBusSubscriptionsManager _subsManager;

        private IModel _consumerChannel;

        public EventBusRabbitMQ(
            IRabbitMQPersistentConnection persistentConnection, 
            ILogger<EventBusRabbitMQ> logger,
            IEventBusSubscriptionsManager subsManager,
            IServiceProvider serviceProvider,
            IOptions<EventBusOptions> options)
        {
            _persistentConnection = persistentConnection;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _options = options.Value;
            _subsManager = subsManager;
            _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
        }

        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            using (var channel = _persistentConnection.CreateModel())
            {
                channel.QueueUnbind(queue: _options.QueueName,
                    exchange: BrokerName,
                    routingKey: eventName);

                if (_subsManager.IsEmpty)
                {
                    _consumerChannel.Close();
                }
            }
        }

        public void Publish(IntegrationEvent @event)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var policy = RetryPolicy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_options.RetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex.ToString());
                });

            using (var channel = _persistentConnection.CreateModel())
            {
                var eventName = @event.GetType()
                    .Name;

                channel.ExchangeDeclare(exchange: BrokerName,
                                    type: "direct");

                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);

                policy.Execute(() =>
                {
                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 2; // persistent

                    channel.BasicPublish(exchange: BrokerName,
                                     routingKey: eventName,
                                     mandatory:true,
                                     basicProperties: properties,
                                     body: body);
                });

                _logger.LogInformation($"Published {@event.GetType().Name}");
            }
        }

        public void SubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler
        {
            DoInternalSubscription(eventName);
            _subsManager.AddDynamicSubscription<TH>(eventName);
        }

        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = _subsManager.GetEventKey<T>();
            DoInternalSubscription(eventName);
            _subsManager.AddSubscription<T, TH>();
        }

        private void DoInternalSubscription(string eventName)
        {
            var containsKey = _subsManager.HasSubscriptionsForEvent(eventName);
            if (!containsKey)
            {
                if (_subsManager.IsEmpty)
                {
                    _consumerChannel = CreateConsumerChannel();
                }

                if (!_persistentConnection.IsConnected)
                {
                    _persistentConnection.TryConnect();
                }

                using (var channel = _persistentConnection.CreateModel())
                {
                    channel.QueueBind(queue: _options.QueueName,
                                      exchange: BrokerName,
                                      routingKey: eventName);
                }
            }
        }

        public void Unsubscribe<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : IntegrationEvent
        {
            _subsManager.RemoveSubscription<T, TH>();
        }

        public void UnsubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler
        {
            _subsManager.RemoveDynamicSubscription<TH>(eventName);
        }

        public void Dispose()
        {
            _consumerChannel?.Dispose();
            _persistentConnection?.Dispose();
            _subsManager.Clear();
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

                await ProcessEvent(eventName, message, ea.Redelivered);

                channel.BasicAck(ea.DeliveryTag,multiple:false);
            };

            channel.BasicConsume(queue: _options.QueueName,
                                 autoAck: false,
                                 consumer: consumer);

            channel.CallbackException += (sender, ea) =>
            {
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
            };

            return channel;
        }

        private async Task ProcessEvent(string eventName, string message, bool redelivered)
        {
            if (_subsManager.HasSubscriptionsForEvent(eventName))
            {
                var subscriptions = _subsManager.GetHandlersForEvent(eventName);
                foreach (var subscription in subscriptions)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var serviceProvider = scope.ServiceProvider;
                        if (subscription.IsDynamic)
                        {
                            if (!(serviceProvider.GetService(subscription.HandlerType) is IDynamicIntegrationEventHandler handler)) continue;
                            dynamic integrationEvent = JObject.Parse(message);
                            await handler.Handle(integrationEvent, redelivered);
                        }
                        else
                        {
                            dynamic handler = serviceProvider.GetService(subscription.HandlerType);
                            if (handler == null) continue;
                            var eventType = _subsManager.GetEventTypeByName(eventName);
                            dynamic integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                            await handler.Handle(integrationEvent, redelivered);
                        }
                    }

                    _logger.LogInformation($"Consumed {eventName}");
                }
            }
        }
    }
}
