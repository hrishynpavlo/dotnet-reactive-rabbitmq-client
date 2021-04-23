using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Reactive.Subjects;

namespace ReactiveRabbitMq.Client.Consumers
{
    public class ReactiveConsumer : BaseReactiveConsumer<ReadOnlyMemory<byte>>, IBasicConsumer
    {
        private readonly Subject<Message<ReadOnlyMemory<byte>>> _subject;
        private readonly IModel _channel;
        private readonly string _queueName;
        private readonly ILogger _logger; 

        public ReactiveConsumer(IModel channel, string queueName, ILoggerFactory loggerFactory)
        {
            _subject = new Subject<Message<ReadOnlyMemory<byte>>>();
            _channel = channel;
            _queueName = queueName;
            _logger = loggerFactory.CreateLogger($"{nameof(ReactiveConsumer)}-{_queueName}");
        }

        public IModel Model => _channel;

        public event EventHandler<ConsumerEventArgs> ConsumerCancelled;

        public override IObservable<Message<ReadOnlyMemory<byte>>> GetMessageStream()
        {
            _channel.BasicConsume(_queueName, true, this);
            return _subject;
        }

        public void HandleBasicCancel(string consumerTag)
        {
            ConsumerCancelled(this, new ConsumerEventArgs(new[] { consumerTag }));
        }

        public void HandleBasicCancelOk(string consumerTag)
        {
            _logger.LogInformation("Consuming channel: {channel} was cancelled, consumer tag: {tag}", _queueName, consumerTag);
        }

        public void HandleBasicConsumeOk(string consumerTag)
        {
            _logger.LogInformation("Consuming channel: {channel} was successfully started, consumer tag: {tag}", _queueName, consumerTag);
        }

        public void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
        {
            _logger.LogDebug("Consumed message from exchange: '{ex}' with routing key: '{rk}', delivery tag: '{tag}'", exchange, routingKey, deliveryTag);
            var message = Message.Create(exchange, routingKey, properties.Headers, body);
            _subject.OnNext(message);
        }

        public void HandleModelShutdown(object model, ShutdownEventArgs reason)
        {
            _logger.LogInformation("Consuming channel: {channel} was shutdown", _queueName);
        }

        public override void Dispose()
        {
            if (!_subject?.IsDisposed ?? false) _subject.Dispose();
            _channel?.Close();
        }
    }
}
