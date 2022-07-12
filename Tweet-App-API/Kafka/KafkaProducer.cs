using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
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
            var conn = "Endpoint=sb://tweet-app-event-hub-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=wePGXfdhFla/mS4/igrixzLHoUeqtStobAB2RVfCBks=";
            var hubname = "tweet-app-event-hub";

            EventHubProducerClient producerClient = new EventHubProducerClient(conn, hubname);

            EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

            eventBatch.TryAdd(new EventData(tweet.Content));

            await producerClient.SendAsync(eventBatch);

            return true;

        }

       
        
    }
}
