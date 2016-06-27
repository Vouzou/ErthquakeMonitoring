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
        /// Gets the earthquakes that happened the last hours depending on the hoursBefore input
        /// </summary>
        /// <param name="hoursBefore"></param>
        /// <returns></returns>
        public static List<EarthquakeData> GetEarthuakeData(int hoursBefore)
        {
            StartTime = DateTime.UtcNow.AddHours(-hoursBefore);
            EndTime = DateTime.UtcNow;
            return GetEarthquakeActivity();
        }
        
        /// <summary>
        /// Calls the service to update the earthquake data
        /// </summary>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public static List<EarthquakeData> UpdateEarthquakeData(DateTime startTime)
        {
            //Update the time for the next iteration
            //Start Time should be the time of the latest earthquake
            StartTime = startTime;
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
