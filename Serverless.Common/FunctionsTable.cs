using Amazon.DynamoDBv2.DataModel;

namespace Serverless.Common
{
    [DynamoDBTable("functions")]
    public class FunctionsTable
    {
        [DynamoDBHashKey]
        public string SubscriptionType { get; set; }

        [DynamoDBRangeKey]
        public string QueueUrl { get; set; }

        public string MatchingInputs { get; set; }

        public string MatchingFunction { get; set; }
    }
}
