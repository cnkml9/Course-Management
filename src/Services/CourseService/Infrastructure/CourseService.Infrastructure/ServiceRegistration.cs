using CourseService.Application.Abstraction.Repositories;
using CourseService.Infrastructure.Concretes.Repositories;
using CourseService.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CourseServiceDbContext>(opt =>
            {
                opt.UseMySql(configuration["ConnectionStrings:CourseDbConnectionString"], ServerVersion.AutoDetect(configuration["ConnectionStrings:CourseDbConnectionString"]));
            });

        }

        public static void AddDependency(this IServiceCollection services)
        {
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<IStudentCourseRepository,StudentCourseRepository>();
        }
    }
}
