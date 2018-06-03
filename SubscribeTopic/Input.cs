using System.Collections.Generic;
using Newtonsoft.Json;

namespace SubscribeTopic
{
    public class Input
    {
        [JsonProperty("subscriberId")]
        public string SubscriberId { get; set; }

        [JsonProperty("topics")]
        public List<string> Topics { get; set; }
    }
}
