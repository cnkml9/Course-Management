using CourseService.Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.DTOs.Teacher
{
    public class CreateCourseViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Decimal Amount { get; set; }
        public string VideoTitle { get; set; }
        public List<IFormFile> File { get; set; }

        public List<string> CourseModelTitle { get; set; }
        public List<string> CourseModelDescription { get; set; }
    }
}
