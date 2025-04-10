namespace IS_Kactus_Expenses.Service
{
    using IS_Kactus_Expenses.Service.Interface;
    using System.Net.Http;

    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;

        public ApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetUserDataAsync(string userCode)
        {
            var response = await _httpClient.GetAsync(userCode);        //En el program se confuguró la URL base

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }

}
