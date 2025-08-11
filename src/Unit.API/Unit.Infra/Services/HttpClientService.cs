namespace Unit.Infra.Services
{
    using Newtonsoft.Json;
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Unit.Application.Services;
    public class HttpClientService : IHttpClient
    {
        readonly HttpClient _httpClient;
        public Uri BaseAddress
        {
            get { return _httpClient.BaseAddress; }
            set { _httpClient.BaseAddress = value; }
        }

        public HttpClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<HttpResponseMessage> DeleteAsync(string requestUri) => SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri));

        public Task<HttpResponseMessage> GetAsync(string requestUri) => SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri));

        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            request.Content = content;
            return SendAsync(request);
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return _httpClient.SendAsync(request);
        }

        public Task<HttpResponseMessage> PostAsJsonAsync<T>(string requestUri, T data)
        {
            string dataAsString = JsonConvert.SerializeObject(data);
            StringContent content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return PostAsync(requestUri, content);
        }

        public void AddDefaultRequestHeader(string name, string value)
        {
            _httpClient.DefaultRequestHeaders.Add(name, value);
        }
    }
}
