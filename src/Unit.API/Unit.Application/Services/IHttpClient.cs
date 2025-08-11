namespace Unit.Application.Services
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    public interface IHttpClient
    {
        Uri BaseAddress { get; set; }
        Task<HttpResponseMessage> GetAsync(string requestUri);
        Task<HttpResponseMessage> DeleteAsync(string requestUri);        
        Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content);
        Task<HttpResponseMessage> PostAsJsonAsync<T>(string requestUri, T data);
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);

        void AddDefaultRequestHeader(string name, string value);
    }
}
