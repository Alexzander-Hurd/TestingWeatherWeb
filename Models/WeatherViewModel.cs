namespace WeatherWeb.Models.DataModels;

public class WeatherViewModel
{
    public List<Period> periods { get; set; }
    public string location { get; set; }
    public string Lat { get; set; }
    public string Long { get; set; }
}
