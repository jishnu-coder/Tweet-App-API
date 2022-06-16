using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tweet_App_API.Kafka
{
    public class KafkaSettings :IKafkaSettings
    {
        public string BootstrapServers { get; set; }
        public string Topic { get; set; }
    }

    public interface IKafkaSettings
    {
        public string BootstrapServers { get; set; }
        public string Topic { get; set; }
    }

}
