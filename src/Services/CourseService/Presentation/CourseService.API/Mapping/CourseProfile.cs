using AutoMapper;
using CourseService.Application.DTOs;
using CourseService.Application.DTOs.Student;
using CourseService.Application.DTOs.Teacher;
using CourseService.Application.DTOs.Videos;
using CourseService.Domain.Models;

namespace CourseService.API.Mapping
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {

            {
                CreateMap<CreateCourseViewModel, Course>();
                CreateMap<Course, CreateCourseViewModel>();

                CreateMap<BuyCourseViewModel, StudentCourse>();
                CreateMap<StudentCourse, BuyCourseViewModel>();

                CreateMap<UpdateCourseViewModel, Course>();
                CreateMap<Course, UpdateCourseViewModel>();

                CreateMap<StudentCourseViewModel, StudentCourse>();
                CreateMap<StudentCourse, StudentCourseViewModel>();

                CreateMap<Video, GetVideoViewModel>();
                CreateMap<GetVideoViewModel, Video>();

                //CreateMap<CreateCourseViewModel, Course>()
                //    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid())) // Id'yi oluştur
                //    .ForMember(dest => dest.Videos, opt => opt.Ignore()) // Videos için ignore (boş liste kullanılacak)
                //    .ForMember(dest => dest.StudentCourses, opt => opt.Ignore()); // StudentCourses için ignore (boş liste kullanılacak)
            }

        }
    }
}
