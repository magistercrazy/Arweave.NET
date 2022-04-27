using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Arweave.NET
{
    public class Client
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly string _baseUrl = "https://arweave.net/";

        public Client()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.BaseAddress = new Uri(_baseUrl);
        }

        public Client(string baseUrl):base()
        {
            _baseUrl = baseUrl;
            _client.BaseAddress = new Uri(_baseUrl);
        }

        public async Task<HttpResponseMessage> Request(HttpRequestMessage requestMessage )
        {
            return await _client.SendAsync(requestMessage);
        }
    }
}
