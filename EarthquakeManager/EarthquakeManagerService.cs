using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Plethora.Managers.EarthquakeManager.Contracts.Service;
using System.ServiceModel;
using GeoJSON.Net.Feature;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using Plethora.Managers.EarthquakeManager.Contracts.Data.DTO;
using Plethora.Managers.EarthquakeManager.Contracts.Data.Request;
using Plethora.Managers.EarthquakeManager.Contracts.Data.Response;

namespace Plethora.Managers.EarthquakeManager
{
    [ServiceBehavior(InstanceContextMode =  InstanceContextMode.PerSession)]
    public class EarthquakeManagerService : IEarthquakeManagerService
    {

        private string _city1 = "";
        private string _city2 = "";
        private string _city3 = "";
        private double _minDist1 = double.MaxValue;
        private double _minDist2 = double.MaxValue;
        private double _minDist3 = double.MaxValue;
        private List<string[]> _cityDataList;

        /// <summary>
        /// Get earthquake activity
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public GetEarthquakeActivityResponse GetEarthquakeActivity(GetEarthquakeActivityRequest request)
        {
            var feedUrl = "http://earthquake.usgs.gov/fdsnws/event/1/query?format=geojson&starttime=" + request.StartTime + "&endtime=" + request.EndTime;
            return GetEarthquakeActivityUtil(feedUrl);
        }
        
        /// <summary>
        /// Utility function to get earthquake acitivity
        /// </summary>
        /// <param name="feedUrl"></param>
        /// <returns></returns>
        private GetEarthquakeActivityResponse GetEarthquakeActivityUtil(string feedUrl)
        {
            var startTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            try
            {
                var geoRequest = WebRequest.Create(feedUrl);
                var geoResponse = geoRequest.GetResponse();
                var dataStream = geoResponse.GetResponseStream();
                if (dataStream == null)
                {
                    return null;
                }
                FeatureCollection featureCollection;
                using (var reader = new StreamReader(dataStream))
                {
                    var data = reader.ReadToEnd();
                    featureCollection = JsonConvert.DeserializeObject<FeatureCollection>(data);
                    reader.Close();
                    geoResponse.Close();
                    if (featureCollection == null)
                    {
                        return null;
                    }
                }
                var earthquakeDataList = new List<EarthquakeData>();
                foreach (var feature in featureCollection.Features)
                {
                    //Reset distances
                    _minDist1 = double.MaxValue;
                    _minDist2 = double.MaxValue;
                    _minDist3 = double.MaxValue;
                    var earthquakeData = new EarthquakeData();
                    var earthquakeTime = feature.Properties["time"];
                    var earthquakeMagnitude = feature.Properties["mag"];
                    var earthquakePoint = feature.Geometry as GeoJSON.Net.Geometry.Point;
                    if (earthquakePoint != null)
                    {
                        var position = earthquakePoint.Coordinates as GeoJSON.Net.Geometry.GeographicPosition;
                        if (position != null)
                        {
                            earthquakeData.Longitude = position.Longitude;
                            earthquakeData.Latitude = position.Latitude;
                            //If it's the first feature we are processing we should generate the list of cities
                            if (_cityDataList == null)
                            {
                                _cityDataList = CreateListOfCities(position.Longitude, position.Latitude);
                            }
                            else
                            {
                                GetAffectedCities(position.Longitude, position.Latitude, _cityDataList);
                            }
                            var cities = new List<string> { _city1, _city2, _city3 };
                            earthquakeData.AffectedCities = cities;
                        }
                    }
                    earthquakeData.DateTime = startTime.AddMilliseconds(long.Parse(earthquakeTime.ToString())).ToLocalTime();
                    earthquakeData.Magnitude = double.Parse(earthquakeMagnitude.ToString());
                    earthquakeDataList.Add(earthquakeData);
                }
                return GetEarthquakeActivityResponse.Create(earthquakeDataList);
            }
            catch (Exception)
            {
                //TODO add logging
                return null;
            }      
        }

        /// <summary>
        /// Creates the list of the _cities from the .csv file
        /// </summary>
        /// <returns></returns>
        private List<string[]> CreateListOfCities(double longitude, double latitude)
        {
            var cityData = new List<string[]>();
            var parser = new TextFieldParser(@"Resources\worldcities.csv");
            //offset to the second line
            parser.ReadLine();
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            while (!parser.EndOfData)
            {
                var fields = parser.ReadFields();
                if (fields != null)
                {
                    var city = fields[6];
                    double lon, lat;
                    if (double.TryParse(fields[8], out lon) && double.TryParse(fields[7], out lat))
                    {
                        var dist = CalculateDistance(longitude, latitude, lon, lat);
                        CheckCityDistanceFromEarthquake(dist, city);
                    }
                }
                cityData.Add(fields);
            }
            parser.Close();
            return cityData;
        }

        /// <summary>
        /// Function to update the affected cities
        /// </summary>
        /// <param name="dist"></param>
        /// <param name="city"></param>
        private void CheckCityDistanceFromEarthquake(double dist, string city)
        {
            if (dist < _minDist1)
            {
                _minDist3 = _minDist2;
                _minDist2 = _minDist1;
                _minDist1 = dist;
                _city3 = _city2;
                _city2 = _city1;
                _city1 = city;
            }
            else if (dist < _minDist2)
            {
                _minDist3 = _minDist2;
                _minDist2 = dist;
                _city3 = _city2;
                _city2 = city;
            }
            else if (dist < _minDist3)
            {
                _minDist3 = dist;
                _city3 = city;
            }
        }

        /// <summary>
        /// Find the affected _cities based on the earthquake's longitude and latitude
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <param name="cityDataList"></param>
        /// <returns></returns>
        private void GetAffectedCities(double longitude, double latitude, List<string[]> cityDataList)
        {
            if (cityDataList != null)
            {
                foreach (var cityData in cityDataList)
                {
                    var city = cityData[6];
                    double lon, lat;
                    if (double.TryParse(cityData[8], out lon) && double.TryParse(cityData[7], out lat))
                    {
                        var dist = CalculateDistance(longitude, latitude, lon, lat);
                        CheckCityDistanceFromEarthquake(dist, city);
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the distance between two longitudes and latitudes
        /// Reference: http://stackoverflow.com/questions/27928/calculate-distance-between-two-latitude-longitude-points-haversine-formula
        /// </summary>
        /// <param name="lon1"></param>
        /// <param name="lat1"></param>
        /// <param name="lon2"></param>
        /// <param name="lat2"></param>
        /// <returns></returns>
        private double CalculateDistance(double lon1, double lat1, double lon2, double lat2)
        {
            //Radus of the earth in km
            const int earthRadius = 6371;
            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);
            var a = Math.Sin(dLat/2)*Math.Sin(dLat/2) +
                    Math.Cos(DegreesToRadians(lat1))*Math.Cos(DegreesToRadians(lat2))*Math.Sin(dLon/2)*Math.Sin(dLon/2);
            var c = 2*Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = earthRadius*c;
            return distance;
        }

        /// <summary>
        /// Converts degrees to radians
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        private double DegreesToRadians(double deg)
        {
            return deg*(Math.PI/180);
        }
    }
}
