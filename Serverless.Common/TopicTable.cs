using Amazon.DynamoDBv2.DataModel;

namespace Serverless.Common
{
    [DynamoDBTable("topics")]
    public class TopicTable
    {
        [DynamoDBHashKey]
        public string TopicName { get; set; }

        [DynamoDBRangeKey]
        public string QueueUrl { get; set; }
    }
}
