using System;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serverless.Common;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace PublishContent
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
            await PublishContents(topics);
            return null;
        }

        public static async Task<bool> PublishContents(Input input)
        {
            var client = ServerlessHelper.GetDbContext();

            foreach (var content in input.Contents)
            {
                var response = client.QueryAsync<ContentTable>(content.Key);
                var items = await response.GetRemainingAsync();
                var message = new
                {
                    content,
                    message = input.Message
                };
                var sqsClient = ServerlessHelper.GetAmazonSqsClient();
                foreach (var item in items)
                {
                    if (item.Key.Equals(content.Key, StringComparison.CurrentCultureIgnoreCase))
                        if (CheckConditions(item.Condition, content.Value, item.Value))
                            await sqsClient.SendMessageAsync(item.QueueUrl, JsonConvert.SerializeObject(message));
                }
            }
            return true;
        }

        private static bool CheckConditions(string condition, string publicationValueString, string subscriberValueString)
        {
            if (condition == ">=" || condition == ">" || condition == "<" || condition == "<=")
            {
                var publicationVal = Double.Parse(publicationValueString);
                var subscriberVal = Double.Parse(subscriberValueString);
                if (Double.IsNaN(publicationVal) || Double.IsNaN(subscriberVal))
                    return false;
                else
                {
                    if (condition == ">=")
                    {
                        if (publicationVal >= subscriberVal)
                            return true;
                    }
                    else if (condition == ">")
                    {
                        if (publicationVal > subscriberVal)
                            return true;
                    }
                    else if (condition == "<")
                    {
                        if (publicationVal < subscriberVal)
                            return true;
                    }
                    else if (condition == "<=")
                    {
                        if (publicationVal <= subscriberVal)
                            return true;
                    }
                }
            }
            else if (condition == "=")
            {
                var publicationVal = publicationValueString;
                var subscriberVal = subscriberValueString;
                if (String.Equals(publicationVal, subscriberVal, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
