using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SQSAPIApplication
{
    public class Backgroundsrvc : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"Running backgroud services started at {DateTime.Now}");
            string acceskey = "AKIAZRURX7WXH6L7G6T5";
            string secretkey = "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";
            var credentials = new BasicAWSCredentials(acceskey, secretkey);
            var client = new AmazonSQSClient(credentials, Amazon.RegionEndpoint.USEast1);

            while (!stoppingToken.IsCancellationRequested)
            {
                var response = new ReceiveMessageRequest()
                {
                    QueueUrl = "https://sqs.us-east-1.amazonaws.com/656361913774/Sample-queue"
                };

                var data = await client.ReceiveMessageAsync(response);
                if (data.Messages.Count > 0)
                {
                    foreach (var d in data.Messages)
                    {
                        Console.WriteLine($"{d}  {DateTime.Now}");
                        Console.WriteLine(d.Body);
                    }
                }

            }
        }
    }
}

