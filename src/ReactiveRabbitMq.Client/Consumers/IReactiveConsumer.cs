using System;

namespace ReactiveRabbitMq.Client.Consumers
{
    public interface IReactiveConsumer
    {
        IObservable<Message<T>> Consume<T>(string queueName);
    }
}
