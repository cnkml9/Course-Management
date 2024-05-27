using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.DTOs.User
{
    public class UpdateUserInfoViewModel
    {
        public string NameSurnname { get; set; }
        public string Mail { get; set; }
        public string UserName { get; set; }
    }
}
