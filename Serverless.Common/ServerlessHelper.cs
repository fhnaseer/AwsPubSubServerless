using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace Serverless.Common
{
    public static class ServerlessHelper
    {
        private static IAmazonSQS GetAmazonSqsClient()
        {
            var credentials = new BasicAWSCredentials(Environment.AccessKey, Environment.SecretKey);
            return new AmazonSQSClient(credentials, RegionEndpoint.EUCentral1);
        }

        private static IDynamoDBContext GetDbContext()
        {
            var config = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 };
            return new DynamoDBContext(new AmazonDynamoDBClient(Environment.AccessKey, Environment.SecretKey), config);
        }

        private static IAmazonDynamoDB GetDb()
        {
            var config = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 };
            return new AmazonDynamoDBClient(Environment.AccessKey, Environment.SecretKey);
        }

        public static async Task<Subscriber> CreateQueue(string queueName)
        {
            var createQueueRequest = new CreateQueueRequest();
            createQueueRequest.QueueName = queueName;
            var attrs = new Dictionary<string, string>();
            attrs.Add(QueueAttributeName.VisibilityTimeout, "10");
            createQueueRequest.Attributes = attrs;
            var sqsClient = GetAmazonSqsClient();
            var response = await sqsClient.CreateQueueAsync(createQueueRequest);
            return new Subscriber { SubscriberId = queueName, QueueUrl = response.QueueUrl };
        }

        public static async Task<bool> SaveSubscriber(Subscriber subscriber)
        {
            var client = GetDbContext();
            await client.SaveAsync(subscriber);
            return true;
        }

        public static async Task<bool> SubscribeTopics(SubscribeTopicsInput topicsInput)
        {
            var client = GetDbContext();
            var subscriber = await client.LoadAsync<Subscriber>(topicsInput.SubscriberId);
            foreach (var topic in topicsInput.Topics)
                await client.SaveAsync(new TopicTable { QueueUrl = subscriber.QueueUrl, TopicName = topic });
            return true;
        }

        public static async Task<bool> PublishTopics(PublishTopicInput input)
        {
            var client = GetDbContext();

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
                    var sqsClient = GetAmazonSqsClient();
                    var res = await sqsClient.SendMessageAsync(item.QueueUrl, Newtonsoft.Json.JsonConvert.SerializeObject(message));
                }
            }
            return true;
        }

        public static async Task<bool> SubscribeContent(SubscribeContentInput contentsInput)
        {
            var client = GetDbContext();
            var subscriber = await client.LoadAsync<Subscriber>(contentsInput.SubscriberId);
            foreach (var content in contentsInput.Contents)
                await client.SaveAsync(new ContentTable { QueueUrl = subscriber.QueueUrl, Key = content.Key, Value = content.Value, Condition = content.Condition });
            return true;
        }

        public static async Task<bool> SubscribeFunctions(SubscribeFunctionsInput functionsInput)
        {
            var client = GetDbContext();
            var subscriber = await client.LoadAsync<Subscriber>(functionsInput.SubscriberId);
            await client.SaveAsync(new FunctionsTable { QueueUrl = subscriber.QueueUrl, SubscriptionType = functionsInput.SubscriptionType, MatchingInputs = functionsInput.MatchingInputs, MatchingFunction = functionsInput.MatchingFunction });
            return true;
        }

        public static async Task<bool> PublishContents(PublishContentInput input)
        {
            var client = GetDbContext();

            foreach (var content in input.Contents)
            {
                var response = client.QueryAsync<ContentTable>(content.Key);
                var items = await response.GetRemainingAsync();
                var message = new
                {
                    content,
                    message = input.Message
                };
                var sqsClient = GetAmazonSqsClient();
                foreach (var item in items)
                {
                    if (item.Key.Equals(content.Key, StringComparison.CurrentCultureIgnoreCase))
                        if (CheckConditions(item.Condition, content.Value, item.Value))
                            await sqsClient.SendMessageAsync(item.QueueUrl, Newtonsoft.Json.JsonConvert.SerializeObject(message));
                }
            }
            return true;
        }

        private static bool CheckConditions(string condition, string publicationValueString, string subscriberValueString)
        {
            if (condition == ">=" || condition == ">" || condition == "<" || condition == "<=")
            {
                var publicationVal = double.Parse(publicationValueString);
                var subscriberVal = double.Parse(subscriberValueString);
                if (double.IsNaN(publicationVal) || double.IsNaN(subscriberVal))
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
                if (string.Equals(publicationVal, subscriberVal, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
