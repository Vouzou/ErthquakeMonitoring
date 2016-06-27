using System.Collections.Generic;
using System.Runtime.Serialization;
using Plethora.Managers.EarthquakeManager.Contracts.Data.DTO;

namespace Plethora.Managers.EarthquakeManager.Contracts.Data.Response
{
    [DataContract]
    public class GetEarthquakeActivityResponse
    {
        #region Static factory

        /// <summary>
        /// Create response
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static GetEarthquakeActivityResponse Create(List<EarthquakeData> data)
        {
            return new GetEarthquakeActivityResponse
            {
                Data = data
            };
        }

        #endregion

        #region Data Members

        /// <summary>
        /// Earthquake Data
        /// </summary>
        [DataMember]
        public List<EarthquakeData> Data { get; set; }

        #endregion
    }
}
