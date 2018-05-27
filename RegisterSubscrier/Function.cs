using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
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
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<string> FunctionHandler(ILambdaContext context)
        {
            var guid = Guid.NewGuid().ToString().Replace("-", "");
            var id = $"subscriber{guid}";
            try
            {
                var response = await ServerlessHelper.CreateQueue(id);
                await ServerlessHelper.SaveSubscriber(response);
                return id;
            }
            catch (Exception e)
            {
                context.Logger.Log(e.Message);
                return null;
            }
        }
    }
}
