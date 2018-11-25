using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.SQS;
using Newtonsoft.Json.Linq;
using Serverless.Common;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace PublishContent
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
            await PublishContents(topics);
            return null;
        }

        private static IAmazonSQS _sqsClient;
        private static IAmazonSQS SqsClient => _sqsClient ?? (_sqsClient = ServerlessHelper.GetAmazonSqsClient());

        public static async Task<bool> PublishContents(Input input)
        {
            var functionInvoked = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var client = ServerlessHelper.GetDbContext();

            var taskList = new List<Task>();
            foreach (var content in input.Contents)
            {
                var response = client.QueryAsync<ContentTable>(content.Key);
                var items = await response.GetRemainingAsync();
                var databaseAccessed = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");

                foreach (var item in items)
                {
                    var payload = new
                    {
                        content,
                        message = input.Message,
                        fromPublisher = input.FromPublisher,
                        functionInvoked = functionInvoked,
                        databaseAccessed = databaseAccessed,
                        queueUrl = item.QueueUrl
                    };
                    if (item.Key.Equals(content.Key, StringComparison.CurrentCultureIgnoreCase))
                        if (CheckConditions(item.Condition, content.Value, item.Value))
                        {
                            LambdaContext.Logger.LogLine($"Matched,");
                            taskList.Add(SqsClient.SendMessageAsync(item.QueueUrl, Newtonsoft.Json.JsonConvert.SerializeObject(payload)));
                        }
                }
            }
            LambdaContext.Logger.Log("sent,");
            await Task.WhenAll(taskList);
            LambdaContext.Logger.Log("done,");
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
