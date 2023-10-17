namespace Common.POCOs;

public class WeatherForecast : POCO
{
    public DateOnly Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }

    public override string ToString() => $"{Date} : {TemperatureF} deg - {Summary}";
}
