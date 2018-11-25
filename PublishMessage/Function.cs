using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.SQS;
using Newtonsoft.Json.Linq;
using Serverless.Common;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace PublishMessage
{
    public class Function
    {
        private static IAmazonSQS _sqsClient;
        private static IAmazonSQS SqsClient
        {
            get { return _sqsClient ?? (_sqsClient = ServerlessHelper.GetAmazonSqsClient()); }
        }

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
                var topics = input.ToObject<Input>();
                context.Logger.LogLine(topics.Message);
                context.Logger.LogLine(topics.QueueUrl);
                await PublishMessage(topics);
                return "Succesful";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static async Task PublishMessage(Input input)
        {
            input.Message = _sqsClient == null ? "Null" : "Not Null";
            await SqsClient.SendMessageAsync(input.QueueUrl, Newtonsoft.Json.JsonConvert.SerializeObject(input));
        }
    }
}
