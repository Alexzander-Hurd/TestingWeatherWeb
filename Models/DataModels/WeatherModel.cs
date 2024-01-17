namespace WeatherWeb.Models.DataModels
{


    public class Period
    {
        public string type;
        public string value;
        public List<WeatherObj> Rep;
    }

    public class WeatherObj
    {
        public string F { get; set; }
        public string G { get; set; }
        public string H { get; set; }
        public string T { get; set; }
        public string V { get; set; }
        public string D { get; set; }
        public string S { get; set; }
        public string U { get; set; }
        public string W { get; set; }
        public string Pp { get; set; }
        public string time { get; set; }
    }
}