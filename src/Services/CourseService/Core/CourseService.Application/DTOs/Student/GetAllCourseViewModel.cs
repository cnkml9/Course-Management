using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.DTOs.Student
{
    public class GetAllCourseViewModel
    {
        public Guid CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string TeacherName { get; set; }
        public int VideoCount { get; set; }
        public decimal Amount { get; set; }
    }
}
