using System;

namespace ReactiveRabbitMq.Client.Consumers
{
    public abstract class BaseReactiveConsumer<T> : IDisposable
    {
        public abstract void Dispose();

        public abstract IObservable<Message<T>> GetMessageStream();
    }
}
