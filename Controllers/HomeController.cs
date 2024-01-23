using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WeatherWeb.Models;
using WeatherWeb.Models.DataModels;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Net;
using System.Text;

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

    public IActionResult Weather(WeatherViewModel data)
    {

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


        WeatherViewModel model;
        if (string.IsNullOrEmpty(data.Lat) || string.IsNullOrEmpty(data.Long))
        {
            model = new WeatherViewModel();
            model.Lat = "53.8498";
            model.Long = "-0.4576";
        }
        else
        {
            if (double.TryParse(data.Lat, out double trash) && double.TryParse(data.Long, out double trash2))
            {
                model = data;
            }
            else
            {
                model = new WeatherViewModel();
                model.Lat = "53.8498";
                model.Long = "-0.4576";

            }
        }


        string closestID = "NA";
        double closest = double.MaxValue;

        using (StreamReader r = new StreamReader("sitelist.json"))
        {
            string json = r.ReadToEnd();
            dynamic obj = JsonConvert.DeserializeObject(json);

            foreach (var site in obj.Locations["Location"])
            {
                double tempLat = site["latitude"];
                double tempLong = site["longitude"];
                double latDiff = Math.Abs(double.Parse(model.Lat) - tempLat);
                double longDiff = Math.Abs(double.Parse(model.Long) - tempLong);
                double distance = Math.Abs(Math.Sqrt((latDiff * latDiff) + (longDiff * longDiff)));

                if (distance < closest)
                {
                    closest = distance;
                    closestID = site["id"];
                }
            }
        }

        string path = "weatherdata/" + closestID + ".json";



        if (!System.IO.File.Exists(path) || System.IO.File.GetLastWriteTimeUtc(path)<= DateTime.UtcNow.Date)
        {
            _logger.LogInformation("Curling page");
            string url = @"http://datapoint.metoffice.gov.uk/public/data/val/wxfcs/all/json/" + closestID + @"?res=3hourly&key=77f59cd6-511a-4558-a286-79641bacacf9";
            _logger.LogInformation(url);
            WebRequest request = WebRequest.Create(url);

            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            using (StreamReader sr = new StreamReader(responseStream, Encoding.UTF8))
            {
                string content = sr.ReadToEnd();

                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.Write(content);
                    sw.Flush();
                    sw.Close();
                }
            }
        }

        using (StreamReader r = new StreamReader(path))
        {
            string json = r.ReadToEnd();
            json = json.Split("DV")[0].Replace("$", "desc") + "DV" + json.Split("DV")[1].Replace("$", "time");

            dynamic obj = JsonConvert.DeserializeObject(json);
            List<Period> periods = new List<Period>();
            model.location = obj.SiteRep.DV.Location["name"].ToString();
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
                }
            }
            model.periods = periods;

            return View(model);
        }
    }

    public IActionResult LatValid(string Lat)
    {
        double dLat;
        double lowerLim = 49.96;
        double upperLim = 60.86;
        if (!double.TryParse(Lat, out dLat))
        {
            return Json($"Please enter a numerical value");
        }

        if (dLat < lowerLim || dLat > upperLim)
        {
            return Json($"Please enter coordinates within the UK");
        }

        return Json(true);
    }

    public IActionResult LongValid(string Long)
    {
        _logger.LogInformation("Entered Validate Lat");

        double dLong;

        double lowerLim = -8.2;
        double upperLim = 1.78;

        if (!double.TryParse(Long, out dLong))
        {
            return Json($"Please enter a numerical value");
        }

        if (dLong < lowerLim || dLong > upperLim)
        {
            return Json($"Please enter coordinates within the UK");
        }
        return Json(true);
    }
}
