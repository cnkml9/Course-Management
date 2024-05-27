using CourseService.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CourseServiceDbContext>(opt =>
            {
                opt.UseMySql(configuration["AuthDbConnectionString"], ServerVersion.AutoDetect(configuration["AuthDbConnectionString"]));
            });

        }
    }
}
