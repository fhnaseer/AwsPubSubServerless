using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.SQS;
using Newtonsoft.Json.Linq;
using Serverless.Common;

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

        private static IAmazonSQS _sqsClient;
        private static IAmazonSQS SqsClient => _sqsClient ?? (_sqsClient = ServerlessHelper.GetAmazonSqsClient());

        public static async Task PublishTopics(Input input)
        {
            var functionInvoked = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var client = ServerlessHelper.GetDbContext();
            var taskList = new List<Task>();
            foreach (var topic in input.Topics)
            {
                var response = client.QueryAsync<TopicTable>(topic);
                var items = await response.GetRemainingAsync();
                var databaseAccessed = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");

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
                    taskList.Add(SqsClient.SendMessageAsync(item.QueueUrl, Newtonsoft.Json.JsonConvert.SerializeObject(payload)));
                }
            }
            LambdaContext.Logger.Log("sent,");
            await Task.WhenAll(taskList);
            LambdaContext.Logger.Log("done,");
        }
    }
}
