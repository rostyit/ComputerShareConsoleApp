using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ComputerShare.Interfaces;
using Newtonsoft.Json;

namespace ComputerShare.Classes
{
    public class ApiCaller : IApiCaller
    {
        private readonly HttpClient _httpClient;

        public ApiCaller(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<byte[]> GetHttpByteArrayAsync(string url)
        {
            var readContents = await ReadContentAsync<byte[]>(
                url,
                async (content) => await content.ReadAsByteArrayAsync(),
                (response, content) => throw new ApiException(url, (int)response.StatusCode, ""));

            return readContents;
        }

        public async Task<T> GetHttpStringContentAsync<T>(string url)
        {
            var readContents = await ReadContentAsync<string>(
                url,
                async (content) => await content.ReadAsStringAsync(),
                (response, content) => throw new ApiException(url, (int)response.StatusCode, content));

            return JsonConvert.DeserializeObject<T>(readContents);
        }

        public async Task<T> PostJsonObject<T>(string url, object contentToSend)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                var json = JsonConvert.SerializeObject(contentToSend);
                using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    request.Content = stringContent;

                    using (var response = await _httpClient
                        .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                        .ConfigureAwait(false))
                    {
                        response.EnsureSuccessStatusCode();
                        var readContents = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<T>(readContents);
                    }
                }
            }
        }

        private async Task<T> ReadContentAsync<T>(
            string url,
            Func<HttpContent, Task<T>> contentHandler,
            Action<HttpResponseMessage, T> exceptionHandler)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                using (var response = await _httpClient.SendAsync(request))
                {
                    var content = await contentHandler(response.Content);

                    if (response.IsSuccessStatusCode == false)
                    {
                        exceptionHandler(response, content);
                    }

                    return content;
                }
            }
        }
    }
}
