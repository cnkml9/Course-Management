using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Domain.Models
{
    public class Course
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid TeacherId { get; set; }


        public bool isActive { get; set; } = false;

        public virtual ICollection<Video> Videos { get; set; } = new List<Video>();
        public virtual ICollection<CourseModel> CourseModels { get; set; } = new List<CourseModel>();

        public virtual ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
    }
}
