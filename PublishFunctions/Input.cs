using Newtonsoft.Json;

namespace PublishFunctions
{
    public class Input
    {
        [JsonProperty("subscriptionTopic")]
        public string SubscriptionTopic { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
