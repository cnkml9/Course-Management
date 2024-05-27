using CourseService.Application.Abstraction.Repositories;
using CourseService.Application.DTOs.Videos;
using CourseService.Domain.Models;
using CourseService.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Infrastructure.Concretes.Repositories
{
    public class VideoRepository : GenericRepository<Video>, IVideoRepository
    {
        public VideoRepository(CourseServiceDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<GetVideoViewModel>> GetVideoByCourseId(Guid courseId)
        {
           var data =  base._context.Videos.ToList().Where(c => c.CourseId == courseId);

            List<GetVideoViewModel> response = new List<GetVideoViewModel>();

            foreach (var video in data)
            {
                GetVideoViewModel getVideoViewModel = new()
                {
                    Id = video.Id,
                    Title = video.Title,
                    Url = video.Url,
                    CourseId = courseId,
                };

                response.Add(getVideoViewModel);
            }

           return response;
        }
    }
}
