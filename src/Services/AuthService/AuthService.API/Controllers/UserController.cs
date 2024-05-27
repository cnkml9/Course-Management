using AuthService.Application.Abstractions.Services.Auth;
using AuthService.Application.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        readonly IAuthService _authService;

        public UserController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPut("UpdateUserInfo")]
        [Authorize]
        public async Task<IActionResult> UpdateUserInfo(UpdateUserInfoViewModel model)
        {
            if (ModelState.IsValid) {

                var response = await _authService.UpdateUserInfo(model);
                if (response != null)
                {
                    return Ok(response);    
                }


            }

            return BadRequest();    
        }
    }
}
