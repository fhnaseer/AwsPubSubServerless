using Newtonsoft.Json;

namespace Serverless.Common
{
    public class Input
    {
        [JsonProperty("subscriptionType")]
        public string SubscriptionType { get; set; }

        [JsonProperty("subscriberId")]
        public string SubscriberId { get; set; }

        [JsonProperty("functionType")]
        public string FunctionType { get; set; }

        [JsonProperty("matchingInputs")]
        public string MatchingInputs { get; set; }

        [JsonProperty("matchingFunction")]
        public string MatchingFunction { get; set; }
    }
}
