using Amazon.DynamoDBv2.DataModel;

namespace Serverless.Common
{
    [DynamoDBTable("subscribers")]
    public class Subscriber
    {
        [DynamoDBHashKey]
        public string SubscriberId { get; set; }
        public string QueueUrl { get; set; }
    }
}
