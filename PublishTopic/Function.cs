using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Newtonsoft.Json.Linq;
using Serverless.Common;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace PublishTopic
{
    public class Function
    {

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<string> FunctionHandler(JObject input, ILambdaContext context)
        {
            var topics = input.ToObject<Input>();
            await PublishTopics(topics);
            return null;
        }

        public static async Task PublishTopics(Input input)
        {
            var client = ServerlessHelper.GetDbContext();
            foreach (var topic in input.Topics)
            {
                var response = client.QueryAsync<TopicTable>(topic);
                var items = await response.GetRemainingAsync();
                var message = new
                {
                    topic,
                    message = input.Message
                };
                foreach (var item in items)
                {
                    var sqsClient = ServerlessHelper.GetAmazonSqsClient();
                    await sqsClient.SendMessageAsync(item.QueueUrl, Newtonsoft.Json.JsonConvert.SerializeObject(message));
                }
            }
        }
    }
}
