using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tweet_App_API.Kafka
{
    public static class KafkaProducer
    {
        public static async Task<bool> KafkaProducerConfig(string message)
        {
            var config = new ProducerConfig { BootstrapServers = "localhost:29092" };

            // If serializers are not specified, default serializers from
            // `Confluent.Kafka.Serializers` will be automatically used where
            // available. Note: by default strings are encoded as UTF8.
            using (var p = new ProducerBuilder<Null, string>(config).Build())
            {
                try
                {
                    var dr = await p.ProduceAsync("test-topic", new Message<Null,string> { Value = message });
                    Console.WriteLine($"Message Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Delivery failed: {e.Message}");

                }
            }
            return false;
        }

       
        
    }
}
