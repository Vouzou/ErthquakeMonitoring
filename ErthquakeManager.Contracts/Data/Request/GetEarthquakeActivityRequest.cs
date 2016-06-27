using System.Runtime.Serialization;

namespace Plethora.Managers.EarthquakeManager.Contracts.Data.Request
{
    [DataContract]
    public class GetEarthquakeActivityRequest
    {
        #region Static factory

        /// <summary>
        /// GetEarthquakeActivity Request
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static GetEarthquakeActivityRequest Create(string startTime, string endTime)
        {
            return new GetEarthquakeActivityRequest
            {
                StartTime = startTime,
                EndTime = endTime
            };
        }

        #endregion

        #region Data members

        /// <summary>
        /// Limit the events on or after the specified start time
        /// </summary>
        [DataMember]
        public string StartTime { get; set; }

        /// <summary>
        /// Limit the events on or before the specified start time
        /// </summary>
        [DataMember]
        public string EndTime { get; set; }

    #endregion
    }
}
