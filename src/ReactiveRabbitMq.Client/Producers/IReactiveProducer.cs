using System.Collections.Generic;

namespace ReactiveRabbitMq.Client.Producers
{
    public interface IReactiveProducer
    {
        void Produce<T>(ProducedDestination destination, T message);
        void Produce<T>(ProducedDestination destination, T message, Dictionary<string, object> headers);
    }
}
