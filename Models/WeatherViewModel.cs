namespace WeatherWeb.Models.DataModels;

public class WeatherViewModel
{
    public List<Period> periods { get; set; }
    public string location { get; set; }

    [Microsoft.AspNetCore.Mvc.Remote(controller:"Home", action:"LatValid")]
    public string Lat { get; set; }

    [Microsoft.AspNetCore.Mvc.Remote(controller:"Home", action:"LongValid")]
    public string Long { get; set; }
}
