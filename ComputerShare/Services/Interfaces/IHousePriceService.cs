
using System.Threading.Tasks;
using ComputerShare.Classes;

namespace ComputerShare.Services.Interfaces
{
    public interface IHousePriceService
    {
        Task<HousePriceInformation> GetHousePriceInfoForLocationAsync(string postcode, double latitude, double longitude);
    }
}
