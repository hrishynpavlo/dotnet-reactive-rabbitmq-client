using Microsoft.Extensions.Logging;
using ReactiveRabbitMq.Client;
using ReactiveRabbitMq.Client.Options;
using Serilog;
using System;

namespace ReactiveRabbitMQ.ClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var isRunning = true;

            var options = new ReactiveClientOptions();

            var serilogger = new LoggerConfiguration().WriteTo.Console().MinimumLevel.Debug().CreateLogger();
            var loggerFactory = new LoggerFactory().AddSerilog(serilogger);

            var client = new ReactiveRabbitMQClient(options, loggerFactory);
            var sub1 = client.Consume<Person>("test-q").Subscribe(x => Console.WriteLine(x.Body));
            Console.CancelKeyPress += new ConsoleCancelEventHandler((_, __) =>
            {
                sub1.Dispose();
                client.Dispose();
                isRunning = false;
                serilogger.Dispose();
                loggerFactory.Dispose();
            });

            while (isRunning)
            {
            }
        }
    }
}
