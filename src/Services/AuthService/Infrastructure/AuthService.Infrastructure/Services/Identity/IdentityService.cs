using AuthService.Application.Abstractions.Services.Identity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Services.Identity
{
    public class IdentityService : IIdentityService
    {
        IHttpContextAccessor httpContextAccessor;
        public IdentityService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
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
