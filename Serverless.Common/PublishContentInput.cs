using System.Collections.Generic;
using Newtonsoft.Json;

namespace Serverless.Common
{
    public class PublishContentInput
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("content")]
        public List<Content> Contents { get; set; }
    }
}
