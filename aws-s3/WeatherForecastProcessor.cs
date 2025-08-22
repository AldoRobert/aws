
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Amazon;

namespace aws_s3
{
    public class WeatherForecastProcessor : BackgroundService  //abstract class , let's implement the abstract class
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Starting Background processor");


                 var config = new ConfigurationBuilder()
                          .SetBasePath(Directory.GetCurrentDirectory())
                          .AddJsonFile("appsettings.json")
                          .Build();

            var accessKey = config.GetValue<string>("AccessKeySQS");
            var secret = config.GetValue<string>("SecretSQS");

            var credentials = new BasicAWSCredentials(accessKey, secret);

            var client = new AmazonSQSClient(credentials, Amazon.RegionEndpoint.USEast2);

          

            while (!stoppingToken.IsCancellationRequested)
            {
                var request = new ReceiveMessageRequest()
                {
                    QueueUrl = "https://sqs.us-east-2.amazonaws.com/050752614353/youtube-demo-sqs"
                };

                var response = await client.ReceiveMessageAsync(request);  

                if (response.Messages != null )
                {
                    foreach (var message in response.Messages)
                    {
                        Console.WriteLine(message.Body);

                    }
                }

               
            }

        }
    }
}
