using System.Collections.Generic;
using Newtonsoft.Json;

namespace Serverless.Common
{
    public class SubscribeTopicsInput
    {
        [JsonProperty("subscriberId")]
        public string SubscriberId { get; set; }


        [JsonProperty("topics")]
        public List<string> Topics { get; set; }
    }
}
