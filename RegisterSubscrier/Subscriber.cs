using Amazon.DynamoDBv2.DataModel;

namespace RegisterSubscrier
{
    public class Subscriber
    {
        [DynamoDBHashKey]
        public string SubscriberId { get; set; }

        public string QueueUrl { get; set; }
    }
}
