using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Amazon.Runtime;
using Amazon.SQS;
using System.Threading.Tasks;

namespace Serverless.Common
{
    public static class ServerlessHelper
    {
        public static IAmazonSQS GetAmazonSqsClient()
        {
            var credentials = new BasicAWSCredentials(Environment.AccessKey, Environment.SecretKey);
            return new AmazonSQSClient(credentials, RegionEndpoint.EUCentral1);
        }

        public static async void ExecuteLambda(string functionName, string payload)
        {
            var credentials = new BasicAWSCredentials(Environment.AccessKey, Environment.SecretKey);
            var client = new AmazonLambdaClient(credentials, RegionEndpoint.EUCentral1);
            var request = new InvokeRequest() { FunctionName = functionName, Payload = payload };
            var response = await client.InvokeAsync(request);
        }

        public static async Task SendMessage(string payload)
        {

            var credentials = new BasicAWSCredentials(Environment.AccessKey, Environment.SecretKey);
            var client = new AmazonLambdaClient(credentials, RegionEndpoint.EUCentral1);
            var request = new InvokeRequest() { FunctionName = "PublishMessage", Payload = payload };
            var response = await client.InvokeAsync(request);
        }

        public static IDynamoDBContext GetDbContext()
        {
            var config = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 };
            return new DynamoDBContext(new AmazonDynamoDBClient(Environment.AccessKey, Environment.SecretKey), config);
        }
    }
}
