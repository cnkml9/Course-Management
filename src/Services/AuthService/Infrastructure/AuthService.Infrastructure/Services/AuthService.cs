using AuthService.Application.Abstractions.Services.Auth;
using AuthService.Application.Abstractions.Services.Identity;
using AuthService.Application.Abstractions.Services.Token;
using AuthService.Application.DTOs.User;
using AuthService.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        readonly UserManager<AppUser> _userManager;
        readonly ITokenHandler _tokenHandler;
        readonly IConfiguration _configuration;
        readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        readonly IIdentityService _identityService;


        public AuthService(UserManager<AppUser> userManager, ITokenHandler tokenHandler, IConfiguration configuration, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager, IIdentityService identityService)
        {
            _userManager = userManager;
            _tokenHandler = tokenHandler;
            _configuration = configuration;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _identityService = identityService;
        }

        public async Task<string> createAsync(CreateUserViewModel model)
        {
            IdentityResult result = await _userManager.CreateAsync(new()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = model.UserName,
                Email = model.Email,
                NameSurnname = model.NameSurname,
                RoleName = model.RoleName,
            }, model.Password);

            //CreateUserResponse response = new()
            //{
            //    Succeded = result.Succeeded
            //};

            if (result.Succeeded)
                return "registration successful";
            else
            {
                //foreach (var error in result.Errors)
                //{
                //    response.Message = $"{error.Code}-{error.Description}\n";
                //}

                return "registration failed";

                throw new Exception("registration failed");

            }

        }


        public async Task<Application.DTOs.Token> LoginAsync(string UserNameOrEmail, string Password, int accesTokenLifeTime)
        {
            AppUser user = await _userManager.FindByNameAsync(UserNameOrEmail);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(UserNameOrEmail);
            }
            if (user == null)
            {
                throw new Exception("User or password incorrect");
            }
            SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, Password, false);

            if (result.Succeeded)
            {
                Application.DTOs.Token token = _tokenHandler.CreateAccessToken(accesTokenLifeTime, user);
                await UpdateRefreshToken(token.refreshToken, user, token.Expiration, 5);

                return token;
            }
            else
            {
                throw new Exception("Login Failed");
            }
            //return new LoginUserErrorCommandResponse()
            //{
            //    Message="Kullanıcı adı veya şifre hatalı..."
            //};
        }

        public async Task<Application.DTOs.Token> RefreshTokenLoginAsync(string refreshToken)
        {
            AppUser? user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user != null && user?.RefreshTokenEndDate > DateTime.UtcNow)
            {
                Application.DTOs.Token token = _tokenHandler.CreateAccessToken(15, user);
                await UpdateRefreshToken(token.refreshToken, user, token.Expiration, 300);
                return token;
            }
            else
                throw new Exception("Hata oluştu");
        }

        public async Task UpdateRefreshToken(string refreshToken, AppUser user, DateTime AccessTokenDate, int addOnAccessTokenDate)
        {
            if (user != null)
            {
                user.RefreshToken = refreshToken;
                user.RefreshTokenEndDate = AccessTokenDate.AddSeconds(addOnAccessTokenDate);
                await _userManager.UpdateAsync(user);
            }
            else
                throw new Exception("Hata oluştu");
        }

        public async Task<string> AddRole( string roleName)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                return ($"Role '{roleName}' already exists");
            }

            AppRole role = new AppRole();
            role.Name = roleName;

            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {

                return $"Role '{roleName}' added successfully";
            }
            else
                return $"Role '{roleName}' added Failed";

        }

        public async Task AssignRole(string userName, string roleName)
        {
            // Rol var mı diye kontrol et
            var roleExists = await _roleManager.RoleExistsAsync(roleName);

            if (!roleExists)
            {
                // Rol yoksa oluştur
                await AddRole(roleName);
            }

            // Kullanıcıya rol ata
            var user = await _userManager.FindByNameAsync(userName);
            await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<string> UpdateUserInfo(UpdateUserInfoViewModel user)
        {
            var userId = _identityService.GetUserId();

            var existingUser = await _userManager.FindByIdAsync(userId);
            if (existingUser == null)
            {
                throw new Exception("User Not Found");
            }

            existingUser.UserName = user.UserName;
            existingUser.Email = user.Mail;
            existingUser.NameSurnname = user.NameSurnname;
            // Diğer güncellenecek bilgileri ekleyin

            var result = await _userManager.UpdateAsync(existingUser);

            if (result.Succeeded)
            {

                return "User information updated successfully";
            }

            else
                return "User information updated failed";

        }

        public async Task<AppUser> GetUserInfoById(string userId)
        {
            // Kullanıcıyı ID'ye göre getir
            var user = await _userManager.FindByIdAsync(userId);

            // Kullanıcı bulunursa, user nesnesini döndür
            return user;
        }

        public async Task<string> ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            try
            {
                var userId = _identityService.GetUserId();

                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return "Kullanıcı bulunamadı.";
                }

                var changePasswordResult = await _userManager.ChangePasswordAsync(user, changePasswordViewModel.CurrentPassword, changePasswordViewModel.NewPassword);

                if (!changePasswordResult.Succeeded)
                {
                    // Şifre değiştirme başarısız oldu, hataları işleyin
                    var errors = changePasswordResult.Errors.Select(e => e.Description);
                    return $"Şifre değiştirme başarısız: {string.Join(", ", errors)}";
                }

                // Şifre değiştirme başarılı
                return "Şifre başarıyla değiştirildi.";
            }
            catch (Exception ex)
            {
                return  $"Bir hata oluştu. Detaylı bilgi için logları kontrol edin. Hata: {ex.Message}";
            }
        }
    }
}
