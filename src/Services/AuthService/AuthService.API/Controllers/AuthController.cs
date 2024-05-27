using AuthService.Application.Abstractions.Services;
using AuthService.Application.Abstractions.Services.Auth;
using AuthService.Application.DTOs;
using AuthService.Application.DTOs.User;
using AuthService.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        readonly IAuthService _authService;
        readonly ILoggerService _logger;


        public AuthController(IAuthService authService, ILoggerService logger)
        {
            _authService = authService;
            _logger = logger;
        }

        //Kullanıcın sisteme login olma işlemi. Burada geriye bir token dönüyor ve bütün servisler 
        //bu token ile sisteme authanticate oluyor
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequestModel request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.UserNameOrEmail) || string.IsNullOrEmpty(request.Password))
                {
                    ModelState.AddModelError("Validation", "Username and password are required fields.");
                    return BadRequest(ModelState);
                }

                //Token üreten kısım
                var token = await _authService.LoginAsync(request.UserNameOrEmail, request.Password, 15);

                if (token == null)
                {
                    ModelState.AddModelError("Login", "Login failed. Incorrect username or password.");
                    return BadRequest(ModelState);
                }

                return Ok(token);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "An error occurred during the login process.");
                _logger.LogError($"Error during login: {ex.Message}");
                return BadRequest(ModelState);
            }
        }


        //Kullanıcının sisteme kayıt olma işlemi
        [HttpPost("Register")]
        public async Task<IActionResult> CreateUser(CreateUserViewModel request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) ||
                    string.IsNullOrEmpty(request.NameSurname) ||
                    string.IsNullOrEmpty(request.Password) ||
                    string.IsNullOrEmpty(request.PasswordConfirm) ||
                    string.IsNullOrEmpty(request.UserName) ||
                    string.IsNullOrEmpty(request.RoleName))
                {
                    ModelState.AddModelError("Validation", "All fields are required.");
                    return BadRequest(ModelState);
                }

                if (request.Password != request.PasswordConfirm)
                {
                    ModelState.AddModelError("Validation", "Password and password confirmation do not match.");
                    return BadRequest(ModelState);
                }

                string response = await _authService.createAsync(new()
                {
                    Email = request.Email,
                    NameSurname = request.NameSurname,
                    Password = request.Password,
                    PasswordConfirm = request.PasswordConfirm,
                    UserName = request.UserName,
                    RoleName = request.RoleName,
                });

                // Rol oluşturma veya kullanıcıya rol değeri atama işlemi
                await _authService.AssignRole(request.UserName, request.RoleName);

                return Ok(response);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "An error occurred during user registration.");
                _logger.LogError($"Error during user registration: {ex.Message}");
                return BadRequest(ModelState);
            }
        }

        //Kullanıcının id ye göre bilgilerini getirme işlemi
        [HttpGet("GetUserById")]
        public async Task<IActionResult> GetUserInfoById(string userId)
        {
            var user = await _authService.GetUserInfoById(userId);

            if (user != null)
            {
                var userName = user.UserName;
                var email = user.Email;

                return Ok(new { UserName = userName, Email = email });
            }
            else
            {
                return NotFound(); 
            }
        }

        //Kullanıcının şifresini değiştirebilme işlemi
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            try
            {
   
                if (string.IsNullOrEmpty(changePasswordViewModel.NewPassword) || changePasswordViewModel.NewPassword.Length < 6)
                {
                    return BadRequest("The new password must be at least 6 characters long.");
                }

                // Perform the password change operation using the AuthService
                var response = await _authService.ChangePassword(changePasswordViewModel);

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Handle errors, perform logging, or other necessary actions here
                Console.Error.WriteLine($"Error during password change: {ex.Message}");
                _logger.LogInformation($"Error during password change: {ex.Message}");

                return StatusCode(500, "An error occurred during password change. Check logs for detailed information.");
            }
        }

    }
}
