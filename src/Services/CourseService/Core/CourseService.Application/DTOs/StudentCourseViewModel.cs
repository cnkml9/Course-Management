using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.DTOs
{
    public class StudentCourseViewModel
    {
        public Guid CourseId { get; set; }
        public string TeacherName { get; set; }
        public string CourseTitle { get; set; }
        public string CourseDescription { get; set; }
        public decimal Amount { get; set; }
        //public CreateCourseViewModel createCourseViewModel { get; set; }
    }
}
