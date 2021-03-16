using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BuienRadarDataSource
{
    public class Buienradar
    {
        [JsonProperty("$id")]
        public string Id { get; set; }

        [JsonProperty("copyright")]
        public string Copyright { get; set; }

        [JsonProperty("terms")]
        public string Terms { get; set; }
    }

    public class Stationmeasurement
    {
        [JsonProperty("$id")]
        public string Id { get; set; }

        [JsonProperty("stationid")]
        public int Stationid { get; set; }

        [JsonProperty("stationname")]
        public string Stationname { get; set; }

        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lon")]
        public double Lon { get; set; }

        [JsonProperty("regio")]
        public string Regio { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("weatherdescription")]
        public string Weatherdescription { get; set; }

        [JsonProperty("iconurl")]
        public string Iconurl { get; set; }

        [JsonProperty("graphUrl")]
        public string GraphUrl { get; set; }

        [JsonProperty("winddirection")]
        public string Winddirection { get; set; }

        [JsonProperty("temperature")]
        public double Temperature { get; set; }

        [JsonProperty("groundtemperature")]
        public double Groundtemperature { get; set; }

        [JsonProperty("feeltemperature")]
        public double Feeltemperature { get; set; }

        [JsonProperty("windgusts")]
        public double Windgusts { get; set; }

        [JsonProperty("windspeed")]
        public double Windspeed { get; set; }

        [JsonProperty("windspeedBft")]
        public int WindspeedBft { get; set; }

        [JsonProperty("humidity")]
        public double Humidity { get; set; }

        [JsonProperty("precipitation")]
        public double Precipitation { get; set; }

        [JsonProperty("sunpower")]
        public double Sunpower { get; set; }

        [JsonProperty("rainFallLast24Hour")]
        public double RainFallLast24Hour { get; set; }

        [JsonProperty("rainFallLastHour")]
        public double RainFallLastHour { get; set; }

        [JsonProperty("winddirectiondegrees")]
        public int Winddirectiondegrees { get; set; }

        [JsonProperty("airpressure")]
        public double? Airpressure { get; set; }

        [JsonProperty("visibility")]
        public double? Visibility { get; set; }
    }

    public class Actual
    {
        [JsonProperty("$id")]
        public string Id { get; set; }

        [JsonProperty("actualradarurl")]
        public string Actualradarurl { get; set; }

        [JsonProperty("sunrise")]
        public DateTime Sunrise { get; set; }

        [JsonProperty("sunset")]
        public DateTime Sunset { get; set; }

        [JsonProperty("stationmeasurements")]
        public List<Stationmeasurement> Stationmeasurements { get; set; }
    }

    public class Weatherreport
    {
        [JsonProperty("$id")]
        public string Id { get; set; }

        [JsonProperty("published")]
        public DateTime Published { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("authorbio")]
        public string Authorbio { get; set; }
    }

    public class Shortterm
    {
        [JsonProperty("$id")]
        public string Id { get; set; }

        [JsonProperty("startdate")]
        public DateTime Startdate { get; set; }

        [JsonProperty("enddate")]
        public DateTime Enddate { get; set; }

        [JsonProperty("forecast")]
        public string Forecast { get; set; }
    }

    public class Longterm
    {
        [JsonProperty("$id")]
        public string Id { get; set; }

        [JsonProperty("startdate")]
        public DateTime Startdate { get; set; }

        [JsonProperty("enddate")]
        public DateTime Enddate { get; set; }

        [JsonProperty("forecast")]
        public string Forecast { get; set; }
    }

    public class Fivedayforecast
    {
        [JsonProperty("$id")]
        public string Id { get; set; }

        [JsonProperty("day")]
        public DateTime Day { get; set; }

        [JsonProperty("mintemperature")]
        public string Mintemperature { get; set; }

        [JsonProperty("maxtemperature")]
        public string Maxtemperature { get; set; }

        [JsonProperty("mintemperatureMax")]
        public int MintemperatureMax { get; set; }

        [JsonProperty("mintemperatureMin")]
        public int MintemperatureMin { get; set; }

        [JsonProperty("maxtemperatureMax")]
        public int MaxtemperatureMax { get; set; }

        [JsonProperty("maxtemperatureMin")]
        public int MaxtemperatureMin { get; set; }

        [JsonProperty("rainChance")]
        public int RainChance { get; set; }

        [JsonProperty("sunChance")]
        public int SunChance { get; set; }

        [JsonProperty("windDirection")]
        public string WindDirection { get; set; }

        [JsonProperty("wind")]
        public int Wind { get; set; }

        [JsonProperty("mmRainMin")]
        public double MmRainMin { get; set; }

        [JsonProperty("mmRainMax")]
        public double MmRainMax { get; set; }

        [JsonProperty("weatherdescription")]
        public string Weatherdescription { get; set; }

        [JsonProperty("iconurl")]
        public string Iconurl { get; set; }
    }

    public class Forecast
    {
        [JsonProperty("$id")]
        public string Id { get; set; }

        [JsonProperty("weatherreport")]
        public Weatherreport Weatherreport { get; set; }

        [JsonProperty("shortterm")]
        public Shortterm Shortterm { get; set; }

        [JsonProperty("longterm")]
        public Longterm Longterm { get; set; }

        [JsonProperty("fivedayforecast")]
        public List<Fivedayforecast> Fivedayforecast { get; set; }
    }

    public class BuienRadarData
    {
        [JsonProperty("$id")]
        public string Id { get; set; }

        [JsonProperty("buienradar")]
        public Buienradar Buienradar { get; set; }

        [JsonProperty("actual")]
        public Actual Actual { get; set; }

        [JsonProperty("forecast")]
        public Forecast Forecast { get; set; }
    }
}