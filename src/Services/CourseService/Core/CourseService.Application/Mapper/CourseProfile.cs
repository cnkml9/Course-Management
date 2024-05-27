//using AutoMapper;
//using CourseService.Application.DTOs;
//using CourseService.Domain.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CourseService.Application.Mapper
//{
//    public class CourseProfile : Profile
//    {
//        public CourseProfile()
//        {

//            {
//                CreateMap<CreateCourseViewModel, Course>();
//                CreateMap<Course, CreateCourseViewModel>();

//                CreateMap<UpdateCourseViewModel, Course>();
//                CreateMap<Course, UpdateCourseViewModel>();

//                CreateMap<StudentCourseViewModel, StudentCourse>();
//                CreateMap<StudentCourse, StudentCourseViewModel>();

//                //CreateMap<CreateCourseViewModel, Course>()
//                //    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid())) // Id'yi oluştur
//                //    .ForMember(dest => dest.Videos, opt => opt.Ignore()) // Videos için ignore (boş liste kullanılacak)
//                //    .ForMember(dest => dest.StudentCourses, opt => opt.Ignore()); // StudentCourses için ignore (boş liste kullanılacak)
//            }

//        }
//    }
//}
