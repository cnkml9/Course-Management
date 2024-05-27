using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.DTOs.Student
{
    public class BuyCourseViewModel
    {
        public Guid StudentId { get; set; }
        public Guid CourseId { get; set; }
    }
}
