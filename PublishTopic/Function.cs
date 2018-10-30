using Amazon.Lambda.Core;
using Newtonsoft.Json.Linq;
using Serverless.Common;
using System;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace PublishTopic
{
    public class Function
    {
        public static ILambdaContext LambdaContext { get; set; }
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<string> FunctionHandler(JObject input, ILambdaContext context)
        {
            LambdaContext = context;
            var topics = input.ToObject<Input>();
            await PublishTopics(topics);
            return null;
        }

        public static async Task PublishTopics(Input input)
        {
            var functionInvoked = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var client = ServerlessHelper.GetDbContext();
            foreach (var topic in input.Topics)
            {
                var response = client.QueryAsync<TopicTable>(topic);
                var databaseAccessed = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
                var items = await response.GetRemainingAsync();
                var sqsClient = ServerlessHelper.GetAmazonSqsClient();

                foreach (var item in items)
                {
                    var payload = new
                    {
                        topic,
                        message = input.Message,
                        fromPublisher = input.FromPublisher,
                        functionInvoked = functionInvoked,
                        databaseAccessed = databaseAccessed,
                        queueUrl = item.QueueUrl
                    };

                    //var content = JsonConvert.SerializeObject(payload);
                    //var httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(1) };
                    //var resp = httpClient.PostAsync("https://0achmjvzf2.execute-api.eu-central-1.amazonaws.com/pubsub/publishmessage", new StringContent(content, System.Text.Encoding.UTF8, "application/json"));
                    //LambdaContext.Logger.LogLine(resp.IsSuccessStatusCode.ToString());
                    //var err = await resp.Content.ReadAsStringAsync();
                    //LambdaContext.Logger.LogLine(err);

                    //var credentials = new BasicAWSCredentials(Serverless.Common.Environment.AccessKey, Serverless.Common.Environment.SecretKey);
                    //var client1 = new AmazonLambdaClient(credentials, RegionEndpoint.EUCentral1);
                    //var request = new InvokeRequest() { FunctionName = "PublishMessage", Payload = JsonConvert.SerializeObject(payload) };
                    //await client1.InvokeAsync(request);
                    //LambdaContext.Logger.LogLine(resp.StatusCode.ToString());
                    //LambdaContext.Logger.LogLine(resp.FunctionError.ToString());
                    //await ServerlessHelper.SendMessage(Newtonsoft.Json.JsonConvert.SerializeObject(payload));
                    await sqsClient.SendMessageAsync(item.QueueUrl, Newtonsoft.Json.JsonConvert.SerializeObject(payload));
                }
            }
        }
    }
}
