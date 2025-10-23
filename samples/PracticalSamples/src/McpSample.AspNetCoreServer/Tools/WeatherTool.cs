using ModelContextProtocol.Server;
using System.ComponentModel;

namespace McpSample.AspNetCoreServer;

[McpServerToolType]
public static class WeatherTool
{
    [McpServerTool, Description("Returns the current weather for a specific city")]
    public static string GetWeatherForCity(string cityName)
    {
        Console.WriteLine("==========================");
        Console.WriteLine($"Function Start WeatherTool: GetWeatherForCity called with cityName: {cityName}");

        // generate a random weather report and return the result
        var random = new Random();
        var temperature = random.Next(-20, 40);
        var condition = random.Next(0, 2) == 0 ? "Sunny" : "Rainy";
        var humidity = random.Next(0, 100);
        var windSpeed = random.Next(0, 20);
        var report = $"Weather in {cityName}: {temperature}°C, {condition}, Humidity: {humidity}%, Wind Speed: {windSpeed} km/h";

        Console.WriteLine("Function report: " + report);
        Console.WriteLine($"Function End WeatherTool: GetWeatherForCity called with cityName: {cityName}");
        Console.WriteLine("==========================");

        return report;
    }
}
