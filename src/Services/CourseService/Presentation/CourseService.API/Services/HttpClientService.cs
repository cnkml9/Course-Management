using CourseService.Application.Abstraction.Services;
using System.Text.Json;
using System.Text;
using System.Net;

namespace CourseService.API.Services
{
    public class HttpClientService : IHttpClientService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpClientService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<HttpStatusCode> PostAsync<TRequest, TResponse>(string url, TRequest requestData)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();

                var requestContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

                using var response = await httpClient.PostAsync(url, requestContent);

               // response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<TResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return response.StatusCode;
            }
            catch (HttpRequestException ex)
            {
                // HTTP isteği sırasında hata oluştuğunda buraya düşer
                Console.Error.WriteLine($"Error during HTTP request: {ex.Message}");
                return HttpStatusCode.InternalServerError; // veya başka bir duruma göre bir StatusCode dönebilirsiniz
            }
            catch (Exception ex)
            {
                // Genel bir hata oluştuğunda buraya düşer
                Console.Error.WriteLine($"Error: {ex.Message}");
                return HttpStatusCode.InternalServerError; // veya başka bir duruma göre bir StatusCode dönebilirsiniz
            }
        }

        public async Task<TResponse> GetAsync<TResponse>(string url)
        {
            using var httpClient = _httpClientFactory.CreateClient();

            using var response = await httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
