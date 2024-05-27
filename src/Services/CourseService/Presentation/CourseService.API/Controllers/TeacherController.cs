using AutoMapper;
using CourseService.API.Services;
using CourseService.Application.Abstraction.Repositories;
using CourseService.Application.Abstraction.Services;
using CourseService.Application.DTOs;
using CourseService.Application.DTOs.Teacher;
using CourseService.Application.DTOs.Videos;
using CourseService.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http;

namespace CourseService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class TeacherController : ControllerBase
    {
        readonly ICourseRepository _courseRepository;
        readonly IIdentityService _identityService;
        readonly IVideoRepository _videoRepository;
        readonly IFileStorageService _fileStorageService;
        private readonly IMapper _mapper;
        readonly ILoggerService _logger;

        public TeacherController(ICourseRepository courseRepository, IIdentityService identityService, IVideoRepository videoRepository, IFileStorageService fileStorageService, IMapper mapper, ILoggerService logger)
        {
            _courseRepository = courseRepository;
            _identityService = identityService;
            _videoRepository = videoRepository;
            _fileStorageService = fileStorageService;
            _mapper = mapper;
            _logger = logger;
        }



        //Öğretmenin kendi yayınlamış olduğu kursları listeleme işlemi yapar 
        [HttpGet("GetCourseByTeacher")]
        public async Task<IActionResult> GetCourseByTeacher()
        {
            if (isRoleTeacher())
            {
                var teacherId = Guid.Parse(_identityService.GetUserId());
                var response = _courseRepository.GetCoursesByTeacherId(teacherId);

                if (response == null || !response.Any())
                {
                    ModelState.AddModelError("Validation", "No courses found for the teacher.");
                    return NotFound(ModelState);
                }

                return Ok(response);
            }

            return BadRequest("Invalid Role");
        }

        //Öğretmenin yayınlamış olduğu kursların videolarını görüntülemesini sağlar.
        [HttpGet("GetCourseTeacherVideoById")]
        public async Task<IActionResult> GetCourseTeacherVideoById(Guid courseId)
        {
            if (isRoleTeacher())
            {

                if (courseId == Guid.Empty)
                {
                    ModelState.AddModelError("Validation", "Invalid courseId.");
                    return BadRequest(ModelState);
                }

                var response = await _videoRepository.GetVideoByCourseId(courseId);

                if (response == null || !response.Any())
                {
                    ModelState.AddModelError("Validation", "No videos found for the specified course.");
                    return NotFound(ModelState);
                }

                return Ok(response);
            }

            return BadRequest("Invalid Role");

        }

        //Öğretmenin kurs yükleme işlemini ve kursa videolarda eklemesini sağlar.(sonradan ekleme-düzenleme yapabilir)
        [HttpPost("CreateCourse")]
        public async Task<ActionResult<Course>> CreateCourse([FromForm
            
            ] CreateCourseViewModel createCourse)
        {
            try
            {

                if (isRoleTeacher())
                {
                    var newCourse = _mapper.Map<Course>(createCourse);
                    var teacherId = Guid.Parse(_identityService.GetUserId());


                    newCourse.TeacherId = teacherId;
                    newCourse.CreatedDate = DateTime.Now;
                    //foreach (var item in  newCourse.CourseModels)
                    //{
                    //    foreach (var item2 in createCourse.CourseModelDescription)
                    //    {
                    //        item.ModelDescription = item2;
                    //    }
                    //    foreach (var item3 in createCourse.CourseModelTitle)
                    //    {
                    //        item.ModelTitle = item3;
                    //    } 
                    //}

                    var response = await _courseRepository.AddAsync(newCourse);

                    List<CourseModel> courseModels = new List<CourseModel>();

                    for (int i = 0; i < createCourse.CourseModelTitle.Count; i++)
                    {
                        courseModels.Add(new CourseModel
                        {
                            CourseModelId = Guid.NewGuid(),
                            ModelTitle = createCourse.CourseModelTitle[i],
                            ModelDescription = createCourse.CourseModelDescription[i],
                            CourseId=response.Id
                        });
                    }

                    if (courseModels != null)
                         await _courseRepository.AddCourseWithCourseModal(courseModels);


                    CreateVideoViewModel vm = new()
                    {
                        Title = createCourse.VideoTitle,
                        File = createCourse.File
                    };



                    await AddVideoToCourse(response.Id, vm);

                    var responseCourse = _mapper.Map<CreateCourseViewModel>(response);
                    responseCourse.VideoTitle = createCourse.VideoTitle;
                    responseCourse.File = createCourse.File;



                    _logger.LogInformation($" Adding a course with courseId {response.Id}  successfully");



                    return Ok(responseCourse);
                }

                return BadRequest("Invalid Role");

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                _logger.LogInformation($" Adding a course with courseId failed message:  {ex}  ");

                return StatusCode(500, new { message = "Internal Server Error" });
            }
        }

        //Öğretmenin kurs bilgilerini güncellemesini sağlar.
        [HttpPut("UpdateCourse")]
        public async Task<ActionResult<Course>> UpdateCourse(UpdateCourseViewModel updateCourse)
        {
            try
            {

                if (isRoleTeacher())
                {
                    var existingCourse = await _courseRepository.GetByIdAsync(updateCourse.Id);

                    if (existingCourse == null)
                    {
                        return NotFound(new { message = "Course not found" });
                    }

                    existingCourse.Amount = updateCourse.Amount;
                    existingCourse.Description = updateCourse.Description;
                    existingCourse.Title = updateCourse.Title;

                    var response = await _courseRepository.UpdateAsync(existingCourse);

                    _logger.LogInformation($" Update a course with courseId {response.Id}  successful");

                    return Ok(response);
                }
                return BadRequest("Invalid Role");

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                _logger.LogInformation($" Update a course with courseId failed message:  {ex}  ");

                return StatusCode(500, new { message = "Internal Server Error" });
            }

        }

        //Öğretmenin kurs silme işlemini sağlar.
        [HttpDelete("DeleteCourse/{courseId}")]
        public async Task<IActionResult> DeleteCourse(Guid courseId)
        {
            try
            {
                if (isRoleTeacher())
                {
                    var existingCourse = await _courseRepository.GetByIdAsync(courseId);

                    if (existingCourse == null)
                    {
                        return NotFound(new { message = "Course not found" });
                    }

                    var GetCourse = await _courseRepository.GetByIdAsync(courseId);

                    if (GetCourse != null)
                    {
                        await _courseRepository.RemoveAsync(GetCourse);
                    }
                    _logger.LogInformation($" Delete a course with courseId {courseId}  successful");

                    return NoContent();
                }
                return BadRequest("Invalid Role");

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                _logger.LogInformation($" Delete a course with courseId failed message:  {ex}  ");

                return StatusCode(500, new { message = "Internal Server Error" });
            }

        }


        //Öğretmenin hazırlamış olduğu kursu yayınlamasını sağlar. Artık kursa öğrenciler erişip satın alabilir.
        [HttpPut("PublishCourse/{courseId}")]
        public async Task<ActionResult> PublishCourse(Guid courseId)
        {
            try
            {
                if (isRoleTeacher())
                {
                    var existingcourse = await _courseRepository.GetByIdAsync(courseId);

                    if (existingcourse == null)
                    {
                        return NotFound("course not found");
                    }

                    existingcourse.isActive = true;

                    await _courseRepository.UpdateAsync(existingcourse);

                    _logger.LogInformation($" Publish a course with courseId {courseId}  successful");

                    return Ok("successful");
                }

                return BadRequest("Invalid Role");

            }

            catch (Exception ex)
            {
                _logger.LogInformation($" Publish a course with courseId failed message:  {ex}  ");

                return StatusCode(500, "An error occurred. Check the logs for more information.");
            }
        }

        //Öğretmenin belirli bir kursa video ekleme işlemini yapar.
        [HttpPost("AddVideoToCourse/{courseId}")]
        public async Task<ActionResult<Video>> AddVideoToCourse(Guid courseId, [FromForm] CreateVideoViewModel video)
        {
            try
            {

                if (isRoleTeacher())
                {
                    foreach (var item in video.File)
                    {
                        var uploadedFileResult = await _fileStorageService.UploadFileAsync(item, courseId);

                        if (uploadedFileResult == null)
                        {
                            return BadRequest("Video yüklenirken bir hata oluştu");
                        }

                        // Yükleme başarılıysa, veritabanına video bilgilerini kaydet
                        var newVideo = new Video
                        {
                            Title = video.Title,
                            Url = uploadedFileResult.Url, // Dosyanın URL'sini kullan
                            CourseId = courseId
                        };

                        await _videoRepository.AddAsync(newVideo);

                    }
                    _logger.LogInformation($" Adding a video with courseId {courseId}  successful");

                    return Ok();
                }

                return BadRequest("Invalid Role");

            }
            catch (Exception ex)
            {
                _logger.LogInformation($" Adding a video with courseId failed message:  {ex}  ");

                return StatusCode(500, "Bir hata oluştu. Detaylı bilgi için logları kontrol edin.");
            }
        }

        //Sisteme login olmuş kullanıcının role değerini getirir (identityService'den getiriyor). 
        private bool isRoleTeacher()
        {
            var userRole = _identityService.GetRoleName();


            if (userRole.ToLower() == "teacher")
                return true;
            else
                return false;

        }

    }
}
