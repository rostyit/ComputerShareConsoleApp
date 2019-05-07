using System.Threading.Tasks;

namespace ComputerShare.Interfaces
{
    public interface IApiCaller
    {
        Task<byte[]> GetHttpByteArrayAsync(string url);
        Task<T> GetHttpStringContentAsync<T>(string url);
        Task<T> PostJsonObject<T>(string url, object contentToSend);
    }
}