using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Plethora.Managers.EarthquakeManager.Contracts.Data.DTO
{
    public class EarthquakeData
    {
        #region Static factory

        #endregion

        #region Data members

        /// <summary>
        /// Date and time of earthquake
        /// </summary>
        [DataMember]
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Magnitude of earthquake
        /// </summary>
        [DataMember]
        public double Magnitude { get; set; }

        /// <summary>
        /// Longitude of earthquake
        /// </summary>
        [DataMember]
        public double Longitude { get; set; }

        /// <summary>
        /// Latitude of earthquake
        /// </summary>
        [DataMember]
        public double Latitude { get; set; }

        /// <summary>
        /// List of affected cities from the earthquake
        /// </summary>
        [DataMember]
        public List<string> AffectedCities { get; set; } 

        #endregion
    }
}
