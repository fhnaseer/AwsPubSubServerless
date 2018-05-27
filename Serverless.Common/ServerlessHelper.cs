using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace Serverless.Common
{
    public static class ServerlessHelper
    {
        private static IAmazonSQS GetAmazonSqsClient()
        {
            var credentials = new BasicAWSCredentials(Environment.AccessKey, Environment.SecretKey);
            return new AmazonSQSClient(credentials, RegionEndpoint.EUCentral1);
        }

        private static IDynamoDBContext GetDbContext()
        {
            var config = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 };
            return new DynamoDBContext(new AmazonDynamoDBClient(Environment.AccessKey, Environment.SecretKey), config);
        }

        public static async Task<Subscriber> CreateQueue(string queueName)
        {
            var createQueueRequest = new CreateQueueRequest();
            createQueueRequest.QueueName = queueName;
            var attrs = new Dictionary<string, string>();
            attrs.Add(QueueAttributeName.VisibilityTimeout, "10");
            createQueueRequest.Attributes = attrs;
            var sqsClient = GetAmazonSqsClient();
            var response = await sqsClient.CreateQueueAsync(createQueueRequest);
            return new Subscriber { SubscriberId = queueName, QueueUrl = response.QueueUrl };
        }

        public static async Task<bool> SaveSubscriber(Subscriber subscriber)
        {
            var client = GetDbContext();
            await client.SaveAsync(subscriber);
            return true;
        }
    }
}
