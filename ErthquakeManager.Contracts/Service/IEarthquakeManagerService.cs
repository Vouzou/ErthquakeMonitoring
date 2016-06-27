using System.ServiceModel;
using Plethora.Managers.EarthquakeManager.Contracts.Data.Request;
using Plethora.Managers.EarthquakeManager.Contracts.Data.Response;

namespace Plethora.Managers.EarthquakeManager.Contracts.Service
{
    [ServiceContract]
    public interface IEarthquakeManagerService
    {
        /// <summary>
        /// Get Earthquake activity
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        GetEarthquakeActivityResponse GetEarthquakeActivity(GetEarthquakeActivityRequest request);
    }
}
