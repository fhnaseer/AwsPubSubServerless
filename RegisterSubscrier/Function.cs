using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Auth.AccessControlPolicy;
using Amazon.Auth.AccessControlPolicy.ActionIdentifiers;
using Amazon.Lambda.Core;
using Amazon.SQS;
using Amazon.SQS.Model;
using Serverless.Common;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace RegisterSubscrier
{
    public class Function
    {
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<Subscriber> FunctionHandler(ILambdaContext context)
        {
            var guid = Guid.NewGuid().ToString().Replace("-", "");
            var id = $"subscriber{guid}";
            try
            {
                var response = await CreateQueue(id);
                return response;
            }
            catch (Exception e)
            {
                context.Logger.Log(e.Message);
                return null;
            }
        }

        private static async Task<Subscriber> CreateQueue(string queueName)
        {
            var createQueueRequest = new CreateQueueRequest();
            createQueueRequest.QueueName = queueName;
            var attrs = new Dictionary<string, string>();
            var policy = new Policy
            {
                Statements = new List<Statement>
                {
                    new Statement(Statement.StatementEffect.Allow)
                    {
                        Principals = new List<Principal>() { Principal.AllUsers },
                        Actions = new List<ActionIdentifier>() { SQSActionIdentifiers.AllSQSActions }
                    }
                }
            };
            attrs.Add(QueueAttributeName.VisibilityTimeout, "10");
            attrs.Add(QueueAttributeName.Policy, policy.ToJson());
            createQueueRequest.Attributes = attrs;
            var sqsClient = ServerlessHelper.GetAmazonSqsClient();
            var response = await sqsClient.CreateQueueAsync(createQueueRequest);
            return new Subscriber { SubscriberId = response.QueueUrl, QueueUrl = response.QueueUrl };
        }

        //private static async void SaveSubscriber(Subscriber subscriber)
        //{
        //    var client = ServerlessHelper.GetDbContext();
        //    await client.SaveAsync(subscriber);
        //}
    }
}
