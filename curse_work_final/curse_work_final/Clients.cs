using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace curse_work_final
{
    public class Clients
    {
        private HttpClient _httpClient;
        private static string _apikey;

        public Clients()
        {
            _apikey = Constants.apikey;
            _httpClient = new HttpClient();
        }

        public async Task<List<GST>> GetGSTAsync(DateTime startDate, DateTime endDate)
        {
            string url = $"{Constants.AddressGST}?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}&api_key={_apikey}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<GST>>(content);
            return result;
        }

        public async Task<List<FLR>> GetFLRAsync(DateTime startDate, DateTime endDate)
        {
            string url = $"{Constants.AddressFLR}?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}&api_key={_apikey}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<FLR>>(content);
            return result;
        }

        public async Task<List<SEP>> GetSEPAsync(DateTime startDate, DateTime endDate)
        {
            string url = $"{Constants.AddressSEP}?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}&api_key={_apikey}";
            Console.WriteLine($"Request URL: {url}");
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<SEP>>(content);
            return result;
        }
    }
}
