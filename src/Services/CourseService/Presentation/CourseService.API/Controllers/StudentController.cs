using AutoMapper;
using CourseService.Application.Abstraction.Repositories;
using CourseService.Application.DTOs;
using CourseService.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using JsonSerializer = System.Text.Json.JsonSerializer;
using CourseService.API.Services;
using Microsoft.AspNetCore.Authorization;
using CourseService.Application.Abstraction.Services;
using System.Net;
using CourseService.Application.DTOs.Student;
using CourseService.Application.DTOs.User;
using Microsoft.IdentityModel.Tokens;
using CourseService.Application.DTOs.Course;

namespace CourseService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        readonly IStudentCourseRepository _studentCourseRepository;
        readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;
        readonly IIdentityService _identityService;
        readonly IHttpClientService _httpClientService;
        private readonly ILoggerService _logger;


        public StudentController(IStudentCourseRepository studentCourseRepository, IMapper mapper, ICourseRepository courseRepository, IIdentityService identityService, IHttpClientService httpClientService, ILoggerService logger)
        {
            _studentCourseRepository = studentCourseRepository;
            _mapper = mapper;
            _courseRepository = courseRepository;
            _identityService = identityService;
            _httpClientService = httpClientService;
            _logger = logger;
        }

        
        //Öğretmenlerin yayınlamış olduğu bütün kursları listeler
        [HttpGet("ListAllCourse")]
        public async Task<IActionResult> ListAllCourse()
        {
            var response = await _courseRepository.GetCourseAll();

            if (response == null || !response.Any())
            {
                ModelState.AddModelError("Validation", "No courses found.");


                return NotFound(ModelState);
            }


            return Ok(response);
        }

        //Öğrencinin satın almış olduğu kursları listeler
        [HttpGet("GetPurchasedCoursesByStudentId")]
        [Authorize]
        public async Task<IActionResult> GetPurchasedCoursesByStudentId()
        {
            try
            {
                if (isRoleStudent())
                {
                    var studentId = Guid.Parse(_identityService.GetUserId());
                    var response = await _studentCourseRepository.GetPurchasedCoursesByStudentId(studentId);

                    if (response == null)
                    {
                        ModelState.AddModelError("Validation", "An error occurred while fetching purchased courses.");
                        return BadRequest(ModelState);
                    }

                    if (!response.Any())
                    {
                        ModelState.AddModelError("Validation", "No courses found.");
                        return NotFound(ModelState);
                    }

                    return Ok(response);
                }

                return BadRequest("Invalid Role");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "An unexpected error occurred.");
                _logger.LogError($"Error during GetPurchasedCoursesByStudentId: {ex.Message}");
                return BadRequest(ModelState);
            }
        }


        //Kursu satın alma işlemini gerçekleştirir. PurchaseService bir istek atar ve orada kurs bilgileri, öğrenci bilgileri vs. tutar
        [HttpPost("BuyCourseFromPurchaseService")]
        [Authorize]
        public async Task<IActionResult> BuyCourseFromPurchaseService( Guid courseId)
        {
            try
            {
                if (isRoleStudent())
                {
                    // Öğrenci kurs satın alma isteğini purchaseService'e gönder
                    var getCourse = await _courseRepository.GetByIdAsync(courseId);
                    var purchaseServiceUrl = "http://localhost:5005/purchase/create";
                    var userName = _identityService.GetUserName();
                    var studentId = _identityService.GetUserId();
                    var requestData = new
                    {
                        userId = studentId,
                        userName,
                        courseId,
                        courseName = getCourse.Title,
                        amount = getCourse.Amount
                    };

                    var requestContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");



                    var result = await _httpClientService.PostAsync<object, PurchaseResult>(purchaseServiceUrl, requestData);

                    // Eğer satın alma başarılı ise, kursa kaydı tamamla
                    if (result == HttpStatusCode.Created)
                    {

                        BuyCourseViewModel buyCourseViewModel = new()
                        {
                            StudentId = Guid.Parse(studentId),
                            CourseId = courseId
                        };

                        await BuyedCourse(buyCourseViewModel);

                        _logger.LogInformation($"studentID:{studentId} courseId: {courseId} course purchase successful");

                        return Ok();
                    }
                    else
                    {
                        _logger.LogInformation($"studentID:{studentId} courseId: {courseId} course purchase failed");
                        return BadRequest();
                    }
                }
                return BadRequest("Invalid Role");
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"courseId: {courseId} course purchase failed");
                Console.Error.WriteLine($"Error during purchase: {ex.Message}");
                return BadRequest();
            }
        }

        //PurchaseService de oluşturduğum payment api ye istek atar ve ödememe doğrulama işlemini yapar
        //Eğer ödeme işlemi success dönerse kurs satın alma işlemini tamamlar isActive değerini true yaparak
        [HttpPost("PaymentCourseFromPurchaseService")]
        [Authorize]
        public async Task<IActionResult> PaymentCourse(PaymentCourseViewModel paymentCourse)
        {
            try
            {
                if (isRoleStudent())
                {

                    //var PaymentServiceUrl = "http://localhost:3009/api/purchase/payment";



                   // var response = await _studentCourseRepository.PaymentFromPurchaseService(paymentCourse.CourseId);

                    var paymentData = new
                    {
                        cardNumber = paymentCourse.CardNumber,
                        expiryDate = paymentCourse.expiryDate,
                        cvv = paymentCourse.CVV
                    };

                   var studentId = _identityService.GetUserId();
                   var  courseId = paymentCourse.CourseId;

                    BuyCourseViewModel buyCourseViewModel = new()
                    {
                        StudentId=Guid.Parse(studentId),
                        CourseId=courseId
                    };

                    await BuyedCourse(buyCourseViewModel);

                    

                  


                        _logger.LogInformation($"  course payment successful");

                        return Ok("Payment Successfuly");
                  
                }
                return BadRequest("Invalid Role");
             
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "An error occurred during course purchase.");
                _logger.LogError($"Error during course purchase: {ex.Message}");
                return BadRequest(ModelState);
            }

           
        }


        //Kurs satın alma işlemi yapıldığında veritabanında ki studentCourse tablosunda student ile kurs değerini eşler.
        [HttpPost("BuyedCourse")]
        [Authorize]
        public async Task<IActionResult> BuyedCourse(BuyCourseViewModel course)
        {

            try
            {
                if (isRoleStudent())
                {
                    if (course.CourseId == Guid.Empty)
                    {
                        ModelState.AddModelError("Validation", "CourseId is required.");
                    }

                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }

                    var newCourse = _mapper.Map<StudentCourse>(course);

                    var studentId = _identityService.GetUserId();

                    newCourse.Id = Guid.NewGuid();

                    var response = await _studentCourseRepository.AddAsync(newCourse);

                    var result = _mapper.Map<BuyCourseViewModel>(response);

                    return Ok(result);
                }
                return BadRequest("Invalid Role");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "An error occurred during course purchase.");
                _logger.LogError($"Error during course purchase: {ex.Message}");
                return BadRequest(ModelState);
            }
        }


        //Kursa ait videoları listeleyecek api 
        [HttpGet("GetVideoByCourse/{courseId}")]
        [Authorize]
        public async Task<IActionResult> GetVideoByCourse(Guid courseId) 
        {
           var response =  await _courseRepository.GetVideosByCourseId(courseId);

            return Ok(response);
        }



        //Sisteme login olmuş kullanıcının role değerini getirir (identityService'den getiriyor). 
        private bool isRoleStudent()
        {
            var userRole = _identityService.GetRoleName();


            if (userRole.ToLower() == "student")
                return true;
            else
                return false;

        }

    }
  


}
