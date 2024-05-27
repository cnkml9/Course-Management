using CourseService.Application.Abstraction.Services;
using CourseService.Application.DTOs.User;
using System.Security.Claims;

namespace CourseService.API.Services
{
    public class IdentityService:IIdentityService
    {
        IHttpContextAccessor httpContextAccessor;
        readonly IHttpClientService _httpClientService;

        public IdentityService(IHttpContextAccessor httpContextAccessor, IHttpClientService httpClientService)
        {
            this.httpContextAccessor = httpContextAccessor;
            _httpClientService = httpClientService;
        }


        public async Task<UserInfoResponse> GetUserInfoById(string userId)
        {
            try
            {
                var apiUrl = $"http://localhost:5005/Auth/GetUserById?userId={userId}";

                // HttpClientService'ı kullanarak GET isteği gönder
                var result = await _httpClientService.GetAsync<UserInfoResponse>(apiUrl);

                UserInfoResponse response = new()
                {
                    UserName = result.UserName,
                    Email = result.Email,
                };

                return response;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error during GET request: {ex.Message}");
                return null;
            }
        }

        public string GetUserName()
        {
            try
            {
                return httpContextAccessor.HttpContext.User.FindFirst(x => x.Type == ClaimTypes.Name).Value;

            }
            catch
            {
                return "Null User";
            }
        }
        public string GetUserId()
        {
            try
            {
                return httpContextAccessor.HttpContext.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value;

            }
            catch
            {
                return "Null User";
            }
        }

        public string GetRoleName()
        {
            try
            {
                return httpContextAccessor.HttpContext.User.FindFirst(x => x.Type == ClaimTypes.Role).Value;

            }
            catch
            {
                return "Null Role";
            }
        }
    }
}
