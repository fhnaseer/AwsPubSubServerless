using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace Serverless.Common
{
    public static class ServerlessHelper
    {
        public static AmazonSQSClient GetAmazonSqsClient()
        {
            var credentials = new BasicAWSCredentials(Environment.AccessKey, Environment.SecretKey);
            return new AmazonSQSClient(credentials, RegionEndpoint.EUCentral1);
        }

        public static async Task<CreateQueueResponse> CreateQueue(string queueName)
        {
            var createQueueRequest = new CreateQueueRequest();
            createQueueRequest.QueueName = queueName;
            var attrs = new Dictionary<string, string>();
            attrs.Add(QueueAttributeName.VisibilityTimeout, "10");
            createQueueRequest.Attributes = attrs;
            var sqsClient = GetAmazonSqsClient();
            var createQueueResponse = await sqsClient.CreateQueueAsync(createQueueRequest);
            return createQueueResponse;
        }
    }
}
