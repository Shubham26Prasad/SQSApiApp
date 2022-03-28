using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using System.IO;
using Amazon;
using Microsoft.Extensions.Configuration;
using System.Collections;
using SQSAPIApplication.Model;
using System.Text.Json;

namespace SQSAPIApplication.Service
{
    public interface IReadSecrets
    {
        secrets Initialization();
    }
    public class ReadSecrets: IReadSecrets
    {
        public IConfiguration _config { get; }
        public ReadSecrets(IConfiguration config)
        {
            _config = config;
        }
        public secrets Initialization()
        {
            MemoryStream my = new MemoryStream();
            IAmazonSecretsManager client = new AmazonSecretsManagerClient(Amazon.RegionEndpoint.USEast1);
            var request = new GetSecretValueRequest()
            {
                SecretId = _config.GetValue<string>("SecretId"),
                VersionStage = "AWSCURRENT"
            };
            GetSecretValueResponse? records = client.GetSecretValueAsync(request).Result;
            var output = JsonSerializer.Deserialize<secrets>(records.SecretString);
            return output;
            //        if(records != null)
            //        {
            //            foreach(var record in records)
            //            {

            //            }
            //        }
            //}


        }
    }
}
