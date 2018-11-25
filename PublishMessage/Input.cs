using Newtonsoft.Json;

namespace PublishMessage
{
    public class Input
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("fromPublisher")]
        public string FromPublisher { get; set; }

        [JsonProperty("functionInvoked")]
        public string FunctionInvoked { get; set; }

        [JsonProperty("databaseAccessed")]
        public string DatabaseAccessed { get; set; }

        [JsonProperty("queueUrl")]
        public string QueueUrl { get; set; }
    }
}
