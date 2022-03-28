using Amazon.SQS;
using Amazon;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.SQS.Model;
using System.Text.Json;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using SQSAPIApplication.Service;
using SQSAPIApplication.Model;

namespace SQSAPIApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        public IConfiguration config { get;  }
        public IReadSecrets _secr { get; set; }
        public secrets Secrets { get; set; }

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration config, IReadSecrets secr)
        {
            _logger = logger;  
            this.config = config;
            _secr = secr;
            this.Secrets = _secr.Initialization();
        }

        [HttpGet("SQSdataRecords")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            secrets scr = _secr.Initialization();
            List<WeatherForecast> wbr = new List<WeatherForecast>(); 
            var acceskey = "AKIAZRURX7WXH6L7G6T5";
            var secretkey = "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";
            var credentials = new BasicAWSCredentials(acceskey, secretkey);
            var client = new AmazonSQSClient(credentials, RegionEndpoint.USEast1);

            var response = new ReceiveMessageRequest()
            {
                QueueUrl = "https://sqs.us-east-1.amazonaws.com/656361913774/Sample-queue",

        };
            while (true)
            {
                var records = await client.ReceiveMessageAsync(response);
                if (records.Messages.Count>0)
                {
                    foreach (var ii in records.Messages)
                    {
                        var rawdata = JsonSerializer.Deserialize<WeatherForecast>(ii.Body);
                        wbr.Add(rawdata);
                    }
                }
                else { break; } 
            }
            
            return wbr;
            
            //var rec = await JsonSerializer.Deserialize<WeatherForecast>(records);
        }
        [HttpPost]
        public async Task Post(WeatherForecast obj)
        {
            string? acceskey = Secrets.acceskey;
            string? secretkey = Secrets.secretkey;
            var credentials = new BasicAWSCredentials(acceskey, secretkey);
            var client = new AmazonSQSClient(credentials, RegionEndpoint.USEast1);
            var request = new SendMessageRequest()
            {
                QueueUrl = Secrets.queueName,
                MessageBody = JsonSerializer.Serialize(obj)

        };
           _=  await client.SendMessageAsync(request);
        }
    }
}
