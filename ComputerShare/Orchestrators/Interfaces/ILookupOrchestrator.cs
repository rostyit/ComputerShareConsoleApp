
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComputerShare.Orchestrators.Interfaces
{
    public interface ILookupOrchestrator
    {
        Task<string> LookupPostcodeDetailsAsync(List<string> postcodes);
    }
}
