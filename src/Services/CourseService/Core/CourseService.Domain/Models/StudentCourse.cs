using System.ComponentModel.DataAnnotations;

namespace CourseService.Domain.Models
{
    public class StudentCourse
    {
        [Key]
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid CourseId { get; set; }
        public bool isActive { get; set; } = false;
        public virtual Course Course { get; set; }
    }
}