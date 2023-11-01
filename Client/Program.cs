using Client;
using Common.Logging;
using Common.POCOs;
using System.Net;

internal class Program
{
    private static readonly FeaturedAPI Connection = new("https://localhost:7238/api/");

    private static readonly APIRequestErrorHandler LoginOn401 = new APIRequestErrorHandler().AddCondition(HttpStatusCode.Unauthorized, async () => await Connection.Login("admin", "root"));
    
    private static async Task Main(string[] args)
    {
        LogWriter.OutputLevel = Common.Logging.LogLevel.DEBUG;

        await GetWeather();

        await Connection.Logout();

        await GetWeather();
        
        Thread.Sleep(1000);
    }

    private static async Task GetWeather()
    {
        var response = await FeaturedAPI.ExecuteWithAutomaticErrorHandler(async () => await Connection.Get<WeatherForecast[]>("WeatherForecast/GetWeatherForecast"), LoginOn401);

        if (response != null && response.IsValid)
        {
            foreach (WeatherForecast forecast in response.Result!)
            {
                LogWriter.LogInfo(forecast);
            }
        }
    }

}