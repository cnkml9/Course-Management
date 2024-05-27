using CourseService.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.Abstraction.Services
{
    public interface IIdentityService
    {
        Task<UserInfoResponse> GetUserInfoById(string userId);
        string GetUserName();
        string GetRoleName();
        string GetUserId();
    }
}
