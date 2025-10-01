using Azure.AI.Agents.Persistent;
using Azure.AI.Projects;
using System.Text.Json;

namespace AgentLabs_02;

public static class CityInfo
{
    // This function is used to get the user's favorite city.
    public static string GetUserFavoriteCity() => "Toronto";
    public static FunctionToolDefinition getUserFavoriteCityTool = new("getUserFavoriteCity", "Gets the user's favorite city.");

    // This function is used to get the nickname of a city.
    public static  string GetWeatherAtLocation(string cityName) => cityName switch
    {
        "Seattle" => "21c",
        "Toronto" => "15c",
        _ => throw new NotImplementedException()
    };
    public static FunctionToolDefinition getWeatherAtLocationTool = new(
    name: "getWeatherAtLocation",
    description: "Gets the weather for a city, e.g. 'Seattle' or 'Toronto'.",
    parameters: BinaryData.FromObjectAsJson(
        new
        {
            Type = "object",
            Properties = new
            {
                CityName = new
                {
                    Type = "string",
                    Description = "The city name",
                },
            },
            Required = new[] { "cityName" },
        },
        new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));

    // this function is used to get the parks of a city
    public static string GetParksAtLocation(string location) => location switch
    {
        "Seattle" => "Wild Waves Theme Park in Seattle offers roller coaster rides1. Visitors can also enjoy other attractions such as water slides, carousel rides, arcade games, and miniature golf3. Another option is Silverwood, a theme park near Coeur D'Alene with both wood and steel roller coasters5",
        "Toronto" => "There are several amusement parks near Toronto with roller coasters and other fun rides. Some options include Canada's Wonderland, Centreville Island Amusement Park, and the Canadian National Exhibition (CNE)",
        _ => throw new NotImplementedException()
    };
    public static FunctionToolDefinition getParksAtLocationTool = new(
name: "getParksAtLocation",
description: "Gets information about parks for a city, e.g. 'Seattle' or 'Toronto'.",
parameters: BinaryData.FromObjectAsJson(
    new
    {
        Type = "object",
        Properties = new
        {
            CityName = new
            {
                Type = "string",
                Description = "The city name",
            },
        },
        Required = new[] { "cityName" },
    },
    new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
}
