using System.Collections.Generic;
using System.Threading.Tasks;
using ComputerShare.Classes;

namespace ComputerShare.Services.Interfaces
{
    public interface IGeocodingService
    {
        Task<List<MapResult>> BulkGeocodePostcodesAsync(List<string> postcode);
    }
}
