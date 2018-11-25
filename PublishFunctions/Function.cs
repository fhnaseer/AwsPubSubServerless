using System.Threading.Tasks;
using Newtonsoft.Json;

using Amazon.Lambda.Core;
using Newtonsoft.Json.Linq;
using Serverless.Common;
using System.Net.Http;
using System.Text;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace PublishFunctions
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
            await PublishFunctions(topics, context);
            return null;
        }

        public static async Task<bool> PublishFunctions(Input input, ILambdaContext context)
        {
            var client = ServerlessHelper.GetDbContext();

            var response = client.QueryAsync<FunctionsTable>(input.SubscriptionTopic);
            var items = await response.GetRemainingAsync();
            var sqsClient = ServerlessHelper.GetAmazonSqsClient();
            var httpClient = new HttpClient();
            context.Logger.Log("database done");
            foreach (var item in items)
            {
                if (item.FunctionType == "url")
                {
                    var resp = await httpClient.GetAsync(item.MatchingFunction);
                    var responseString = await resp.Content.ReadAsStringAsync();
                    await sqsClient.SendMessageAsync(item.QueueUrl, JsonConvert.SerializeObject(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(responseString))));
                }
            }
            return true;
        }
    }
}
