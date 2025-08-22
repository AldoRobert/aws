using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using aws_s3;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<WeatherForecastProcessor>();    //register the Backgropund Class, it starts automatically after registration
                                                                  //it register the class as a hosted services and register the class as a background service


//dynamoDB
var config = new ConfigurationBuilder()
                         .SetBasePath(Directory.GetCurrentDirectory())
                         .AddJsonFile("appsettings.json")
                         .Build();

var accessKey = config.GetValue<string>("AccessKeyDynamoDB");
var secret = config.GetValue<string>("SecretDynamoDB");

var credentials = new BasicAWSCredentials(accessKey, secret);

var _config = new AmazonDynamoDBConfig()
{
    RegionEndpoint = RegionEndpoint.USEast2
};

var client = new AmazonDynamoDBClient(credentials, _config);

builder.Services.AddSingleton<IAmazonDynamoDB>(client);

builder.Services.AddSingleton<IDynamoDBContext, DynamoDBContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
