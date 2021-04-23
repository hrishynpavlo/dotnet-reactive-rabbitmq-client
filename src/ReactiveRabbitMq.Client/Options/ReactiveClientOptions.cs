using System;

namespace ReactiveRabbitMq.Client.Options
{
    public class ReactiveClientOptions
    {
        public string Host { get; set; } = "localhost";
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string VirtualHost { get; set; } = "/";
        public uint Port { get; set; } = 5672;
        public string ClientName { get; set; } = $"TestProj{Guid.NewGuid()}";
    }
}
