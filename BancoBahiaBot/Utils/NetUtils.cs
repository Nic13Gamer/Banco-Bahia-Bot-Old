using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BancoBahiaBot.Utils
{
    class NetUtils
    {
        public static readonly HttpClient httpClient = new();

        public static async Task<HttpResponse> ApiRequest(string url, Dictionary<string, string> content)
        {
            var encodedContent = new FormUrlEncodedContent(content);
            encodedContent.Headers.Add("key", Bot.API_KEY);

            var httpResponse = await httpClient.PostAsync(url, encodedContent);

            HttpResponse response = new((int)httpResponse.StatusCode, await httpResponse.Content.ReadAsStringAsync());

            return response;
        }
    }

    class HttpResponse
    {
        public HttpResponse(int status, string content)
        {
            this.status = status;
            this.content = content;
        }

        public int status;
        public string content;
    }
}
