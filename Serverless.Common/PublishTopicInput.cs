using System.Collections.Generic;
using Newtonsoft.Json;

namespace Serverless.Common
{
    public class PublishTopicInput
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("topics")]
        public List<string> Topics { get; set; }
    }
}
