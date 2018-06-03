using System;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Newtonsoft.Json.Linq;
using Serverless.Common;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace SubscribeContent
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
                await SubscribeContent(input.ToObject<Input>());
                return "Successful";
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        private static async Task SubscribeContent(Input contentsInput)
        {
            var client = ServerlessHelper.GetDbContext();
            foreach (var content in contentsInput.Contents)
                await client.SaveAsync(new ContentTable { QueueUrl = contentsInput.SubscriberId, Key = content.Key, Value = content.Value, Condition = content.Condition });
        }
    }
}
