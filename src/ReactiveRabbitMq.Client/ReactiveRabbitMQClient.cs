using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using ReactiveRabbitMq.Client.Consumers;
using ReactiveRabbitMq.Client.Options;
using ReactiveRabbitMq.Client.Producers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text.Json;

namespace ReactiveRabbitMq.Client
{
    public class ReactiveRabbitMQClient : IReactiveProducer, IReactiveConsumer, IDisposable
    {
        private readonly IConnection _connection;
        private readonly ConcurrentDictionary<ProducedDestination, IModel> _producers;
        private readonly ConcurrentDictionary<string, BaseReactiveConsumer<ReadOnlyMemory<byte>>> _consumers;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<ReactiveRabbitMQClient> _logger;

        public ReactiveRabbitMQClient(ReactiveClientOptions options, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<ReactiveRabbitMQClient>();
            _connection = CreateConnection(options);
            _producers = new ConcurrentDictionary<ProducedDestination, IModel>();
            _consumers = new ConcurrentDictionary<string, BaseReactiveConsumer<ReadOnlyMemory<byte>>>();
        }

        public IObservable<Message<T>> Consume<T>(string queueName)
        {
            var consumer = _consumers.GetOrAdd(queueName, _ => new ReactiveConsumer(_connection.CreateModel(), queueName, _loggerFactory));
            return consumer.GetMessageStream().Select(x =>
            {
                var json = JsonSerializer.Deserialize<T>(x.Body.Span, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return Message.Create(x.Exchange, x.RoutingKey, x.Headers, json);
            });
        }

        public void Produce<T>(ProducedDestination destination, T message)
        {
            Produce(destination, message, null);
        }

        public void Produce<T>(ProducedDestination destination, T message, Dictionary<string, object> headers)
        {
            var producer = _producers.GetOrAdd(destination, _ => _connection.CreateModel());
            var props = producer.CreateBasicProperties();
            props.Headers = headers;

            var bytes = JsonSerializer.SerializeToUtf8Bytes(message).AsMemory();

            producer.BasicPublish(destination.ExchangeName, destination.QueueName, props, bytes);
        }

        public void Dispose()
        {
            try
            {
                foreach (var producer in _producers?.Values)
                {
                    producer.Dispose();
                }

                foreach (var consumer in _consumers?.Values)
                {
                    consumer.Dispose();
                }

                _producers?.Clear();
                _consumers?.Clear();

                _connection?.Close();
                _logger.LogInformation("Successfully closed connection: '{0}'", _connection.ClientProvidedName);
            }
            catch(Exception e)
            {

            }
        }

        private IConnection CreateConnection(ReactiveClientOptions options)
        {
            if(_connection == null)
            {
                var factory = new ConnectionFactory()
                {
                    HostName = options.Host,
                    UserName = options.UserName,
                    Password = options.Password,
                    VirtualHost = options.VirtualHost,
                    ClientProvidedName = options.ClientName
                };

                var connection = factory.CreateConnection();

                _logger.LogInformation("Successfully created connection: '{0}'", connection.ClientProvidedName);

                return connection;
            }

            throw new Exception();
        }
    }
}
