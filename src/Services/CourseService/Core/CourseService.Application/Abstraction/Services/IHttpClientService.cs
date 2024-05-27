using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.Abstraction.Services
{
    public interface IHttpClientService
    {
        Task<HttpStatusCode> PostAsync<TRequest, TResponse>(string url, TRequest data);
        Task<TResponse> GetAsync<TResponse>(string url);
    }
}
