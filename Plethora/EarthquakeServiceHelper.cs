using System;
using System.Collections.Generic;
using Plethora.Managers.EarthquakeManager;
using Plethora.Managers.EarthquakeManager.Contracts.Data.DTO;
using Plethora.Managers.EarthquakeManager.Contracts.Data.Request;
using Plethora.Managers.EarthquakeManager.Contracts.Service;

namespace Plethora
{
    public static class EarthquakeServiceHelper
    {
        internal static IEarthquakeManagerService EarthquakeManagerService = new EarthquakeManagerService();
        public static DateTime StartTime, EndTime;

        /// <summary>
        /// Calls the service to initialize the earthquake data
        /// Gets the earthquakes that happened the last hour
        /// </summary>
        /// <returns></returns>
        public static List<EarthquakeData> GetEarthuakeData()
        {
            var hour = DateTime.UtcNow.Hour - 1;
            var day = DateTime.UtcNow.Day;
            var month = DateTime.UtcNow.Month;
            var year = DateTime.UtcNow.Year;
            if (hour < 0)
            {
                day--;
                hour += 24;
            }
            if (day <= 0)
            {
                month--;
                if (month > 0)
                {
                    day = DateTime.DaysInMonth(year, month);
                }
            }
            if (month <= 0)
            {
                month = 12;
                year--;
                day = DateTime.DaysInMonth(year, month);
            }
            StartTime = new DateTime(year, month, day, hour, DateTime.UtcNow.Minute, DateTime.UtcNow.Second, DateTimeKind.Utc);
            EndTime = DateTime.UtcNow;
            return GetEarthquakeActivity();
        }
        
        /// <summary>
        /// Calls the service to update the earthquake data
        /// </summary>
        /// <returns></returns>
        public static List<EarthquakeData> UpdateEarthquakeData()
        {
            //Update the time for the next iteration
            StartTime = EndTime;
            EndTime = DateTime.UtcNow;
            return GetEarthquakeActivity();
        }

        /// <summary>
        /// Calls the service to get the earthquake data
        /// </summary>
        /// <returns></returns>
        private static List<EarthquakeData> GetEarthquakeActivity()
        {
            var response =
                EarthquakeManagerService.GetEarthquakeActivity(
                    GetEarthquakeActivityRequest.Create(StartTime.ToString("yyyy-MM-ddTHH\\:mm\\:ss"), EndTime.ToString("yyyy-MM-ddTHH\\:mm\\:ss")));
            if (response == null)
                return null;
            return response.Data;
        }
    }
}
