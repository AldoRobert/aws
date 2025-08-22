using Amazon.DynamoDBv2.DataModel;

namespace aws_s3
{
    [DynamoDBTable("WeatherForecast")]
    public class WeatherForecastCity
    {
        [DynamoDBHashKey]
        public string Cty { get; set; }

        [DynamoDBRangeKey]
        public DateTime Date { get; set; }
        [DynamoDBProperty]
        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        [DynamoDBProperty]
        public string? Summary { get; set; }
    }
}
