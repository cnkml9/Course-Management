using AutoMapper;
using CourseService.Application.Abstraction.Repositories;
using CourseService.Application.Abstraction.Services;
using CourseService.Application.DTOs;
using CourseService.Application.DTOs.Course;
using CourseService.Application.DTOs.Teacher;
using CourseService.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO.Compression;

namespace CourseService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {

        readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _hostingEnvironment;
        readonly ILoggerService _logger;
        public CourseController(ICourseRepository courseRepository, IMapper mapper, IWebHostEnvironment hostingEnvironment, ILoggerService logger)
        {
            _courseRepository = courseRepository;
            _mapper = mapper;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        //Kullanıcıların kursu indirebilmelerini sağlar. Kurs için oluşturulmuş dosyayı ve içindeki videoları zip olarak indirir.
        [Authorize]
        [HttpGet("DownloadCourse/{courseId}")]
        public IActionResult DownloadCourse(Guid courseId)
        {
            try
            {
                // Kursun içeriğini saklanan klasör yolu
                var webRootPath = _hostingEnvironment.WebRootPath;

                string courseFolderPath = Path.Combine(webRootPath, courseId.ToString());

                // Kurs klasörü var mı kontrol et
                if (!Directory.Exists(courseFolderPath))
                {
                    return NotFound("Course not found.");
                }

                // Kurs içeriği ZIP dosyası olarak indirilecek
                string zipFilePath = Path.Combine(Path.GetTempPath(), $"{courseId}_CourseContent.zip");

                // ZIP dosyası oluştur ve içeriği ekleyerek sıkıştır
                ZipFile.CreateFromDirectory(courseFolderPath, zipFilePath);

                // ZIP dosyasını kullanıcıya indir
                var memoryStream = new MemoryStream();
                using (var stream = new FileStream(zipFilePath, FileMode.Open))
                {
                    stream.CopyTo(memoryStream);
                }

                memoryStream.Position = 0;

                _logger.LogInformation($" Publish a course with courseId {courseId}  succesfully ");

                return File(memoryStream, "application/zip", $"{courseId}_CourseContent.zip");
            }
            catch (Exception ex)
            {
                _logger.LogInformation($" Publish a course with courseId {courseId}  failed message: {ex}");

                return StatusCode(500, new { message = "Internal Server Error" });
            }

        }

        [HttpGet("GetCourseById")]
        public async Task<IActionResult> GetCourseById(Guid courseId)
        {
            try
            {
                var response = await _courseRepository.GetCourseById(courseId);       

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest("Not find course");
                throw;
            }
        }
    }
}
