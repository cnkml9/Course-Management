using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Domain.Models
{
    public class CourseModel
    {
        [Key]
        public Guid CourseModelId { get; set; }
        public string ModelTitle { get; set; }
        public string ModelDescription { get; set; }
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
    }
}
