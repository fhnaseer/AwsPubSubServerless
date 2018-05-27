using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace Serverless.Common
{
    public static class ServerlessHelper
    {
        private const string SubscribersTableName = "subscribers";
        private const string SubscriberIdColumn = "SubscriberId";
        private const string QueueUrlColumn = "QueueUrl";

        private static IAmazonSQS GetAmazonSqsClient()
        {
            var credentials = new BasicAWSCredentials(Environment.AccessKey, Environment.SecretKey);
            return new AmazonSQSClient(credentials, RegionEndpoint.EUCentral1);
        }

        private static IDynamoDBContext GetDbContext()
        {
            var config = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 };
            return new DynamoDBContext(new AmazonDynamoDBClient(), config);
        }

        public static async Task<CreateQueueResponse> CreateQueue(string queueName)
        {
            var createQueueRequest = new CreateQueueRequest();
            createQueueRequest.QueueName = queueName;
            var attrs = new Dictionary<string, string>();
            attrs.Add(QueueAttributeName.VisibilityTimeout, "10");
            createQueueRequest.Attributes = attrs;
            var sqsClient = GetAmazonSqsClient();
            var response = await sqsClient.CreateQueueAsync(createQueueRequest);
            await SaveSubscriber(queueName, response.QueueUrl);
            return response;
        }

        private static async Task<bool> SaveSubscriber(string subscriberId, string queueUrl)
        {
            var client = new AmazonDynamoDBClient(Environment.AccessKey, Environment.SecretKey);
            var db = GetDbContext();
            var request = new PutItemRequest
            {
                TableName = SubscribersTableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    { SubscriberIdColumn, new AttributeValue { S = subscriberId }},
                    { QueueUrlColumn, new AttributeValue { S = queueUrl }},
                }
            };
            await client.PutItemAsync(request);
            return true;
        }
    }
}
