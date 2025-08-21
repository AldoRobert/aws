using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace aws_s3.Controllers
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

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }


        public string BucketName = "weather-forecast-youtube-demo";

        public string BucketName2 = "youtube-robert-demo-bucket";

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }


        /*
         *   1.- No me permite crear bucket desde robert-aws-vs2022
         * 
         *   2.- le agregue una inline policy con full access al S3 y entonces permite crear un archivo, no permite crear bucket
         * 
         *    3.- create files and folder in buckets
         *    
         *    4.- accessKey and secret from appsettings.json
         *    
         *    5.- Delete a file from a Bucket
         *    
         *    TODO: 6.- Add Dependency Injection for s3Client 
         *    
         *    7.- SNS create a message in a topic (SNS).
         */





        [HttpPost]
        public async Task Post(IFormFile formFile)
        {



            //4.- accessKey and secret from appsettings.json
            var config = new ConfigurationBuilder()
                           .SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("appsettings.json")
                           .Build();

            var accessKey = config.GetValue<string>("AccessKey");
            var secret = config.GetValue<string>("Secret");

            var credentials = new BasicAWSCredentials(accessKey, secret);  //credenciales de robert-aws-vs2022


            var client = new AmazonS3Client(credentials, Amazon.RegionEndpoint.USEast2);  //will use the default profile in AWS Explorer


            //no permite crear bucket
            //var bucketRequest = new PutBucketRequest()
            //{
            //    BucketName = BucketName,
            //    UseClientRegion = true
            //};

            //await client.PutBucketAsync(bucketRequest);

            //upload a file to the bucket
            // 3.- create files and folder in buckets
            var objectRequest = new PutObjectRequest()
            {
                BucketName = "youtube-robert-demo-bucket",
                Key = $"{DateTime.Now:yyyy\\/MM\\/dd\\/hhmmss}-{formFile.FileName}",
                InputStream = formFile.OpenReadStream()
            };

            var response = await client.PutObjectAsync(objectRequest);
        }

        //to retrieve the file
        [HttpGet("GetFile")]
        public async Task<IActionResult> GetFile(string fileName)
        {
            //4.- accessKey and secret from appsettings.json
            var config = new ConfigurationBuilder()
                           .SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("appsettings.json")
                           .Build();

            var accessKey = config.GetValue<string>("AccessKey");
            var secret = config.GetValue<string>("Secret");

            var credentials = new BasicAWSCredentials(accessKey, secret);
            var client = new AmazonS3Client(credentials, Amazon.RegionEndpoint.USEast2);

            var response = await client.GetObjectAsync("youtube-robert-demo-bucket", fileName);


            //para ver el contenido del file 
            using var reader = new StreamReader(response.ResponseStream);

            var fileContents = await reader.ReadToEndAsync();


            return File(response.ResponseStream, response.Headers.ContentType);
        }

        //Retrieves a list of files
        [HttpGet("GetFiles")]
        public async Task<IActionResult> GetFiles(string prefix)
        {
            //4.- accessKey and secret from appsettings.json
            var config = new ConfigurationBuilder()
                           .SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("appsettings.json")
                           .Build();

            var accessKey = config.GetValue<string>("AccessKey");
            var secret = config.GetValue<string>("Secret");

            var credentials = new BasicAWSCredentials(accessKey, secret);
            var client = new AmazonS3Client(credentials, Amazon.RegionEndpoint.USEast2);

            var request = new ListObjectsV2Request()
            {
                BucketName = BucketName2,
                Prefix = prefix
            };

            var response = await client.ListObjectsV2Async(request);

            //TODO: read the content of the first file.


            return Ok();
        }

        //5.- Delete a file from a Bucket

        [HttpDelete]
        public async Task Delete(string fileName)
        {
            var config = new ConfigurationBuilder()
                          .SetBasePath(Directory.GetCurrentDirectory())
                          .AddJsonFile("appsettings.json")
                          .Build();

            var accessKey = config.GetValue<string>("AccessKey");
            var secret = config.GetValue<string>("Secret");

            var credentials = new BasicAWSCredentials(accessKey, secret);
            var client = new AmazonS3Client(credentials, Amazon.RegionEndpoint.USEast2);

            await client.DeleteObjectAsync(BucketName2, fileName);
        }

        //7.- SNS create a message in a topic(SNS).
        [HttpPost("PostSNS")]
        public async Task PostSNS(WeatherForecast data)
        {
            var config = new ConfigurationBuilder()
                          .SetBasePath(Directory.GetCurrentDirectory())
                          .AddJsonFile("appsettings.json")
                          .Build();

            var accessKey = config.GetValue<string>("AccessKey");
            var secret = config.GetValue<string>("Secret");

            var credentials = new BasicAWSCredentials(accessKey, secret);

            var client = new AmazonSimpleNotificationServiceClient(credentials, Amazon.RegionEndpoint.USEast2);

            var request = new PublishRequest()
            {
                TopicArn = "arn:aws:sns:us-east-2:050752614353:youtube-sns",
                Message = JsonSerializer.Serialize(data),
                Subject = "NewWeatherDataAdded"
            };

            var response = await client.PublishAsync(request);
        }
    }

}
