using System;

namespace Plethora
{
    public class EarthquakeDataModel
    {
        public DateTime DateTime { get; set; }
        public double Magnitude { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string ClosestPlaces { get; set; }
    }
}
