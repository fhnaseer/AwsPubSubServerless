using System.Collections.Generic;
using Newtonsoft.Json;

namespace PublishContent
{
    public class Input
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("fromPublisher")]
        public string FromPublisher { get; set; }

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
