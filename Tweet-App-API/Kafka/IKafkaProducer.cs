using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_App_API.Model;

namespace Tweet_App_API.Kafka
{
    public interface IKafkaProducer
    {
        public Task<bool> KafkaProducerConfig(Tweet tweet);
    }
}
