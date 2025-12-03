using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Library.Clinic.Utilities
{
    public class WebRequestHandler
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl = "http://localhost:5000/api"; // Update this with your actual API URL

        public WebRequestHandler()
        {
            _client = new HttpClient();
        }

        public async Task<string> Get(string url)
        {
            var response = await _client.GetAsync($"{_baseUrl}{url}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> Post(string url, object data)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(data),
                Encoding.UTF8,
                "application/json");

            var response = await _client.PostAsync($"{_baseUrl}{url}", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> Delete(string url)
        {
            var response = await _client.DeleteAsync($"{_baseUrl}{url}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
