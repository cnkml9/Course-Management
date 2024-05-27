using CourseService.Application.DTOs.Course;
using CourseService.Application.DTOs.Student;
using CourseService.Application.DTOs.Teacher;
using CourseService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.Abstraction.Repositories
{
    public interface ICourseRepository:IGenericRepository<Course>
    {
        List<CourseDetailViewModel> GetCoursesByTeacherId(Guid TeacherId);
       // Task<CourseDetailViewModel> GetCourseDetails(Guid CourseId);
        Task<List<Application.DTOs.Student.GetAllCourseViewModel>> GetCourseAll();
        Task<GetCourseDetailsById> GetCourseById(Guid Id);
        Task<bool> AddCourseWithCourseModal(List<CourseModel> courseModel);
        Task<List<Video>> GetVideosByCourseId(Guid courseId);
    }
}
