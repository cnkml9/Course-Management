using CourseService.Application.Abstraction.Repositories;
using CourseService.Application.Abstraction.Services;
using CourseService.Application.DTOs;
using CourseService.Application.DTOs.Course;
using CourseService.Domain.Models;
using CourseService.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Infrastructure.Concretes.Repositories
{
    public class StudentCourseRepository : GenericRepository<StudentCourse>, IStudentCourseRepository
    {

        readonly IIdentityService _identityService;

        public StudentCourseRepository(CourseServiceDbContext context, IIdentityService identityService) : base(context)
        {
            _identityService = identityService;
        }

        public async Task<List<StudentCourseViewModel>> GetPurchasedCoursesByStudentId(Guid studentId)
        {
            // Öğrencinin satın aldığı kursları getir
            var response =  base._context.StudentCourses
             .Include(s=>s.Course)
             .Where(s => s.StudentId == studentId)
            .ToList().Distinct();

            List<StudentCourseViewModel> studentCourseViewModels = new List<StudentCourseViewModel>();  


            foreach (var studentCourse in response)
            {

                var teacherId =studentCourse.Course.TeacherId.ToString();

                var teacherInfo = await _identityService.GetUserInfoById(teacherId);


                studentCourseViewModels.Add(new StudentCourseViewModel()
                {
                    CourseId = studentCourse.CourseId,
                    CourseTitle = studentCourse.Course.Title,
                    CourseDescription = studentCourse.Course.Description,
                    TeacherName = teacherInfo.UserName,
                    Amount = studentCourse.Course.Amount
                });
            }

            return studentCourseViewModels;

        }

     
        public async Task<StudentCourse> PaymentFromPurchaseService(Guid Id)
        {
            var response = await base._context.StudentCourses
                .Include(s => s.Course)
                .Where(s => s.CourseId == Id )
                .FirstOrDefaultAsync();

            return response;
        }
    }
}
