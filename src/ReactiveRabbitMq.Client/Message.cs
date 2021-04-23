using System.Collections.Generic;

namespace ReactiveRabbitMq.Client
{
    public class Message<T>
    {
        public Message(string exchange, string routingKey, IDictionary<string, object> headers, T body)
        {
            Exchange = exchange;
            RoutingKey = routingKey;
            Headers = headers == null ? new Dictionary<string, object>() : new Dictionary<string, object>(headers);
            Body = body;
        }

        public string Exchange { get; }
        public string RoutingKey { get; }
        public Dictionary<string, object> Headers { get; }
        public T Body { get; }
    }

    public struct Message
    {
        public static Message<T> Create<T>(string exchange, string routingKey, IDictionary<string, object> headers, T body) => new Message<T>(exchange, routingKey, headers, body);
    }
}
