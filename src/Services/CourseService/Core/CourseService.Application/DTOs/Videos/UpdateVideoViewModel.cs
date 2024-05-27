using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.DTOs.Videos
{
    public class UpdateVideoViewModel
    {
        public string Title { get; set; }
        public IFormFile File { get; set; }
    }
}
