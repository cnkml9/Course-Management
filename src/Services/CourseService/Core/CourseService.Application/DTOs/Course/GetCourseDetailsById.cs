using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.DTOs.Course
{
    public class GetCourseDetailsById
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid TeacherId { get; set; }
        public string TeacherName { get; set; }
        public int VideoCount { get; set; }
        public decimal Amount { get; set; }

        public List<string> ModelTitle { get; set; }
        public List<string> ModelDescription { get; set; }
    }
}
