using System;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Newtonsoft.Json.Linq;
using Serverless.Common;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace SubscribeTopic
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
            try
            {
                await SubscribeTopics(input.ToObject<Input>());
                return "Successful";
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        private static async Task SubscribeTopics(Input input)
        {
            var client = ServerlessHelper.GetDbContext();
            foreach (var topic in input.Topics)
                await client.SaveAsync(new TopicTable { QueueUrl = input.SubscriberId, TopicName = topic });
        }
    }
}
