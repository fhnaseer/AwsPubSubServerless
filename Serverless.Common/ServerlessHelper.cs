using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using Amazon.SQS;

namespace Serverless.Common
{
    public static class ServerlessHelper
    {
        public static IAmazonSQS GetAmazonSqsClient()
        {
            var credentials = new BasicAWSCredentials(Environment.AccessKey, Environment.SecretKey);
            return new AmazonSQSClient(credentials, RegionEndpoint.EUCentral1);
        }

        public static IDynamoDBContext GetDbContext()
        {
            var config = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 };
            return new DynamoDBContext(new AmazonDynamoDBClient(Environment.AccessKey, Environment.SecretKey), config);
        }
    }
}
