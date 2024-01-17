using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WeatherWeb.Models;
using WeatherWeb.Models.DataModels;
using Newtonsoft.Json;
using System.ComponentModel;

namespace WeatherWeb.Controllers;

public class HomeController : Controller
{


    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult Weather()
    {
        WeatherViewModel model = new WeatherViewModel();
        Dictionary<string, string> Visibility = new Dictionary<string, string>()
        {
            {"UN", "Unknown)"},
            {"VP", "Very Poor (<1km)"},
            {"PO", "Poor (1-4km)"},
            {"MO", "Moderate (4-10km)"},
            {"GO", "Good (10-20km)"},
            {"VG", "Very Good (20-40km)"},
            {"EX", "Excellent (>40k)m"}
        };

        Dictionary<string, string> Weather = new Dictionary<string, string>()
        {
            {"NA", "Not available"},
            {"-1", "Trace rain"},
            {"0", "Clear night"},
            {"1", "Sunny day"},
            {"2", "Partly cloudy (night)"},
            {"3", "Partly cloudy (day)"},
            {"5", "Mist"},
            {"6", "Fog"},
            {"7", "Cloudy"},
            {"8", "Overcast"},
            {"9", "Light rain shower (night)"},
            {"10", "Light rain shower (day)"},
            {"11", "Drizzle"},
            {"12", "Light rain"},
            {"13", "Heavy rain shower (night)"},
            {"14", "Heavy rain shower (day)"},
            {"15", "Heavy rain"},
            {"16", "Sleet rain shower (night)"},
            {"17", "Sleet rain shower (day)"},
            {"18", "Sleet"},
            {"19", "Hail rain shower (night)"},
            {"20", "Hail rain shower (day)"},
            {"21", "Hail"},
            {"22", "Light snow shower (night)"},
            {"23", "Light snow shower (day)"},
            {"24", "Light snow"},
            {"25", "Heavy snow shower (night)"},
            {"26", "Heavy snow shower (day)"},
            {"27", "Heavy snow"},
            {"28", "Thunder shower (night)"},
            {"29", "Thunder shower (day)"},
            {"30", "Thunder"}
        };

        using (StreamReader r = new StreamReader("weather.json"))
        {
            string json = r.ReadToEnd();
            json = json.Split("DV")[0].Replace("$", "desc") + "DV" + json.Split("DV")[1].Replace("$", "time");

            dynamic obj = JsonConvert.DeserializeObject(json);
            List<Period> periods = new List<Period>();
            foreach (var rPeriod in obj.SiteRep.DV.Location["Period"])
            {
                periods.Add(JsonConvert.DeserializeObject<Period>(rPeriod.ToString()));
            }
            foreach (var period in periods)
            {
                foreach (var rep in period.Rep)
                {
                    rep.F += "℃";
                    rep.G += "mph";
                    rep.H += "%";
                    rep.T += "℃";
                    rep.S += "mph";
                    rep.Pp += "%";

                    rep.time = (int.Parse(rep.time) / 60).ToString() + ":" + (int.Parse(rep.time) % 60).ToString();
                    if (rep.time.Split(':')[1] == "0")
                    {
                        rep.time += 0;
                    }
                    rep.V = Visibility[rep.V];
                    rep.W = Weather[rep.W];
                }
            }
            model.periods = periods;

            return View(model);
        }
    }
}
