using CourseService.Application.Abstraction.Repositories;
using CourseService.Application.Abstraction.Services;
using CourseService.Application.DTOs;
using CourseService.Application.DTOs.Course;
using CourseService.Application.DTOs.Student;
using CourseService.Application.DTOs.Teacher;
using CourseService.Domain.Models;
using CourseService.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Infrastructure.Concretes.Repositories
{
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        readonly IIdentityService _identityService;
        public CourseRepository(CourseServiceDbContext context, IIdentityService identityService) : base(context)
        {
            _identityService = identityService;
        }

        public List<CourseDetailViewModel> GetCoursesByTeacherId(Guid TeacherId)
        {
            // Öğrencinin satın aldığı kursları getir
            var response = base._context.Courses.Include(v=>v.Videos).Include(s=>s.StudentCourses).
             Where(s => s.TeacherId == TeacherId)
            .ToList();

            List<CourseDetailViewModel> courseDetailViewModels = new List<CourseDetailViewModel> ();

            foreach (var item in response)
            {
                CourseDetailViewModel dto = new()
                {
                    CourseId = item.Id,
                    CourseTitle = item.Title,
                    CourseDescription = item.Description,
                    CourseStudentCount = item.StudentCourses.Count(),
                    CourseVideoCount = item.Videos.Count(),
                    CreatedDate = item.CreatedDate,
                    Amount = item.Amount
                };

                courseDetailViewModels.Add(dto);
            }

            return courseDetailViewModels;

        }
        public async Task<List<Application.DTOs.Student.GetAllCourseViewModel>> GetCourseAll()
        {
          var getAllCourse =   base._context.Courses.Include(v=>v.Videos).ToList();


            List<Application.DTOs.Student.GetAllCourseViewModel> getAllCourseViewModel = new List<Application.DTOs.Student.GetAllCourseViewModel>();


            foreach (var item in getAllCourse)
            {
                var teacherId = item.TeacherId.ToString();

                var teacherInfo = await _identityService.GetUserInfoById(teacherId);

                Application.DTOs.Student.GetAllCourseViewModel CourseDTO = new()
                {
                    CourseId = item.Id,
                    Title = item.Title,
                    Description = item.Description,
                    VideoCount = item.Videos.Count(),
                    Amount = item.Amount,
                    TeacherName = teacherInfo.UserName
                };

                getAllCourseViewModel.Add(CourseDTO);
            }

            return getAllCourseViewModel;
        } 

        public async Task<GetCourseDetailsById>  GetCourseById(Guid Id)
        {
            var getCourse =await base._context.Courses.Include(c=>c.Videos).Include(c=>c.CourseModels).Where(x=>x.Id == Id).FirstOrDefaultAsync();
            var teacherId = getCourse.TeacherId.ToString();

            var teacherInfo = await _identityService.GetUserInfoById(teacherId);
            GetCourseDetailsById getAllCourseDetail = new()
            {
                Id=getCourse.Id,
                Title=getCourse.Title,
                Description=getCourse.Description,
                TeacherId=getCourse.TeacherId,
                ModelTitle= getCourse.CourseModels.Select(cm => cm.ModelTitle).ToList(),
                ModelDescription = getCourse.CourseModels.Select(cm => cm.ModelDescription).ToList(),
                TeacherName=teacherInfo.UserName,
                VideoCount=getCourse.Videos.Count(),
                Amount=getCourse.Amount
            };

            return getAllCourseDetail;

        }

        public async Task<List<Video>> GetVideosByCourseId(Guid courseId)
        {
          var response =  await base._context.Videos.Where(cs => cs.CourseId == courseId).ToListAsync();

           return response;
        }

        public async Task<bool> AddCourseWithCourseModal(List<CourseModel> courseModel)
        {
            try
            {
                await base._context.CourseModels.AddRangeAsync(courseModel);
                return true;

            }
            catch (Exception)
            {
                return false;
                throw;
            }


        }
    }
}
