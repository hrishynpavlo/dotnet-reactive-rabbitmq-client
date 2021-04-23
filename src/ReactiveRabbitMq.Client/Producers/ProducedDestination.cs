using System;

namespace ReactiveRabbitMq.Client.Producers
{
    public class ProducedDestination : IEquatable<ProducedDestination>
    {
        private ProducedDestination(string queueName, string exchangeName)
        {
            QueueName = queueName;
            ExchangeName = exchangeName;
        }

        public string QueueName { get; }
        public string ExchangeName { get; }

        public static ProducedDestination Create(string queueName, string exchangeName) => new ProducedDestination(queueName, exchangeName);
        public static ProducedDestination Queue(string queueName) => new ProducedDestination(queueName, string.Empty);
        public static ProducedDestination Exchange(string exchangeName) => new ProducedDestination(string.Empty, exchangeName);

        public bool Equals(ProducedDestination other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            return ExchangeName == other.ExchangeName && QueueName == other.QueueName;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ProducedDestination);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(QueueName, ExchangeName);
        }

        public override string ToString()
        {
            return $"{QueueName}:{ExchangeName}";
        }
    }
}
