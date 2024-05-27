using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Abstractions.Services.Identity
{
    public interface IIdentityService
    {
        string GetUserName();
        string GetRoleName();
        string GetUserId();
    }
}
