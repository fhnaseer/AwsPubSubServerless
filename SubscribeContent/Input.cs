using System.Collections.Generic;
using Newtonsoft.Json;

namespace SubscribeContent
{
    public class Input
    {
        [JsonProperty("subscriberId")]
        public string SubscriberId { get; set; }

        [JsonProperty("content")]
        public List<Content> Contents { get; set; }
    }

    public class Content
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("condition")]
        public string Condition { get; set; }
    }
}
