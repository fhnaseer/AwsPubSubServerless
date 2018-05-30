using Amazon.DynamoDBv2.DataModel;

namespace Serverless.Common
{
    [DynamoDBTable("content")]
    public class ContentTable
    {
        [DynamoDBHashKey]
        public string Key { get; set; }

        [DynamoDBRangeKey]
        public string QueueUrl { get; set; }

        public string Value { get; set; }

        public string Condition { get; set; }
    }
}
