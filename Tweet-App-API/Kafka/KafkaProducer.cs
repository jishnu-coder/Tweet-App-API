using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tweet_App_API.Model;

namespace Tweet_App_API.Kafka
{
  

    public class KafkaProducer : IKafkaProducer
    {
        private readonly IKafkaSettings _KafkaSettings;

        public KafkaProducer(IKafkaSettings kafkaSettings)
        {
            _KafkaSettings = kafkaSettings;
        }
        public  async Task<bool> KafkaProducerConfig(Tweet  tweet)
        {
            var config = new ProducerConfig { BootstrapServers = _KafkaSettings.BootstrapServers } ;

            // If serializers are not specified, default serializers from
            // `Confluent.Kafka.Serializers` will be automatically used where
            // available. Note: by default strings are encoded as UTF8.
            using (var p = new ProducerBuilder<Null, string>(config).Build())
            {
                try
                {
                    var dr = await p.ProduceAsync(_KafkaSettings.Topic, new Message<Null,string> { Value = tweet.Content });
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
