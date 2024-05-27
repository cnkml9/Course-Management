using CourseService.Application.DTOs;
using CourseService.Application.DTOs.Course;
using CourseService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.Abstraction.Repositories
{
    public interface IStudentCourseRepository:IGenericRepository<StudentCourse>
    {
        Task<List<StudentCourseViewModel>> GetPurchasedCoursesByStudentId(Guid studentId);
        Task<StudentCourse> PaymentFromPurchaseService(Guid Id);
    }
}
