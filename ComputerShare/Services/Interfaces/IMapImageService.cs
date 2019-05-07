using System.Threading.Tasks;

namespace ComputerShare.Services.Interfaces
{
    public interface IMapImageService
    {
        bool LatAndLongRequiredByService { get; }    // Some mapping services may only need the postcode...
        Task<string> GetMapImageForLocationAsync(string postCode, double latitude, double longitude);
    }
}
