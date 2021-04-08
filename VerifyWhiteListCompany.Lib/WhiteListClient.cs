using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VerifyWhiteListCompany.Lib.WebServiceModel;

namespace VerifyWhiteListCompany.Lib
{
    class WhiteListClient : IWhiteListClient
    {
        private const string _dateFormat = "yyyy-MM-dd";
        private  const string _getCompaniesByNipMethodUrl = "api/search/nips";
        private HttpClient _httpClient;
        private bool _isInitiated = false;

        public void Init(string url)
        {
            if (!_isInitiated)
            {
                _httpClient = new HttpClient();
                _httpClient.BaseAddress = new Uri(url);
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                _isInitiated = true;
            }
        }
     

        /// <summary>
        /// Verifies companies of the given nips
        /// </summary>
        /// <param name="nips">Comma separted Nips</param>
        /// <returns></returns>
        public async Task<EntryListResponse> VerifyCompanies(string nips)
        {
            string currentDate = DateTime.Now.ToString(_dateFormat);
            EntryListResponse entities = null;
            string content;
            string getCompaniesByNipUrl = string.Format("{0}/{1}?date={2}", _getCompaniesByNipMethodUrl, nips, currentDate);

            HttpResponseMessage responseMsg = _httpClient.GetAsync(getCompaniesByNipUrl).GetAwaiter().GetResult();
            if (responseMsg.IsSuccessStatusCode)
            {
                content = await responseMsg.Content.ReadAsStringAsync();
                entities = JsonConvert.DeserializeObject<EntryListResponse>(content);

            }
            else
            {
                content = await responseMsg.Content.ReadAsStringAsync();
                var exception = JsonConvert.DeserializeObject<WebServiceModel.Exception>(content);
                throw new WhiteListClientException(exception);
            }



            return entities;
        }
    }
}
