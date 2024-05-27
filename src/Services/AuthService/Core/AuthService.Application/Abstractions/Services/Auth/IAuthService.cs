using AuthService.Application.DTOs.User;
using AuthService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Abstractions.Services.Auth
{
    public interface IAuthService
    {
        Task<DTOs.Token> LoginAsync(string UserNameOrEmail, string Password, int accesTokenLifeTime);
        Task<DTOs.Token> RefreshTokenLoginAsync(string refreshToken);
        Task<string> createAsync(CreateUserViewModel model);
        Task<string> AddRole( string role);
        Task AssignRole(string userName, string roleName);
        Task<AppUser> GetUserInfoById(string userId);
        Task<string> UpdateUserInfo(UpdateUserInfoViewModel user);

        Task<string> ChangePassword(ChangePasswordViewModel changePasswordViewModel);
    }
}
