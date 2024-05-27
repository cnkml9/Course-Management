using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Models
{
    public class AppRole : IdentityRole<string>
    {
        public AppRole() : base()
        {
            Id = Guid.NewGuid().ToString(); // GUID tipinde bir Id oluştur
        }


    }
}
