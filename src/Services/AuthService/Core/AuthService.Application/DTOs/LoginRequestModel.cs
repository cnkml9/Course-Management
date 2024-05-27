using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.DTOs
{
    public class LoginRequestModel
    {
        public string UserNameOrEmail { get; set; }
        public string Password { get; set; }
    }
}
