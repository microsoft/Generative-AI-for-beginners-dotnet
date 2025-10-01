using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace SKFunctions01;

public class CityTemperaturePlugIn
{
    [KernelFunction, Description("Returns the current temperature for a city.")]
    public async Task<string> GetCityTemperature(
        Kernel kernel,
        [Description("The city to check the temperature")] string city
    )
    {
        Console.WriteLine($"== FUNCTION CALL START ==");
        Console.WriteLine($"== City: {city}");

        var random = new Random();
        var temperature = random.Next(-10, 10);
        var message = $"The current temperature in {city} is {temperature} C";

        Console.WriteLine($"== Generated message: {message}");
        Console.WriteLine($"== FUNCTION CALL END ==");
        return message;
    }
}