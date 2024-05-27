using CourseService.Application.DTOs.Videos;
using CourseService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.Abstraction.Repositories
{
    public interface IVideoRepository:IGenericRepository<Video>
    {
        Task<IEnumerable<GetVideoViewModel>> GetVideoByCourseId(Guid courseId);
    }
}
