using System.Text.Json;

namespace ReactiveRabbitMQ.ClientTest
{
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, WriteIndented = true });
        }
    }
}
