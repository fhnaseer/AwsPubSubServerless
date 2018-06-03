using System.Collections.Generic;
using Newtonsoft.Json;

namespace PublishTopic
{
    public class Input
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("topics")]
        public List<string> Topics { get; set; }
    }
}
