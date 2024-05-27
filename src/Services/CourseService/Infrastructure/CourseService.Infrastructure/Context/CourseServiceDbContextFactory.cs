using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Infrastructure.Context
{
    public class CourseServiceDbContextFactory : IDesignTimeDbContextFactory<CourseServiceDbContext>
    {
        public CourseServiceDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CourseServiceDbContext>();
            optionsBuilder.UseMySql("Server=localhost;Database=CourseDb;User=root;Password=Kmlcn323.;", ServerVersion.AutoDetect(("Server=localhost;Database=CourseDb;User=root;Password=Kmlcn323.;")));

            return new CourseServiceDbContext(optionsBuilder.Options);
        }
    }

}
