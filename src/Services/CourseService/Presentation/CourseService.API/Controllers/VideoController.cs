using AutoMapper;
using CourseService.Application.Abstraction.Repositories;
using CourseService.Application.Abstraction.Services;
using CourseService.Application.DTOs.Videos;
using CourseService.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CourseService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class VideoController : ControllerBase
    {
        readonly IVideoRepository _videoRepository;
        readonly IFileStorageService _fileStorageService;
        readonly ILoggerService _logger;
        readonly IIdentityService _identityService;

        public VideoController(IVideoRepository videoRepository, IFileStorageService fileStorageService, ILoggerService logger, IIdentityService identityService)
        {
            _videoRepository = videoRepository;
            _fileStorageService = fileStorageService;
            _logger = logger;
            _identityService = identityService;
        }


        //Öğretmenin belirli bir kurstan belirli bir videoyu silme işlemini yapar. 
        [HttpDelete("DeleteVideoFromCourse/{id}")]
        public async Task<IActionResult> DeleteVideoFromCourse(Guid id)
        {
            try
            {
                if (isRoleTeacher())
                {
                    var getVideo = await _videoRepository.GetByIdAsync(id);

                    if (getVideo != null)
                    {
                        await _videoRepository.RemoveAsync(getVideo);
                        _logger.LogInformation($" Delete a video with videoId {id}  successful");

                    }
                    else
                    {
                        return BadRequest();
                    }
                    return NoContent();
                }

                return BadRequest("Invalid Role");
            }
            catch (Exception ex)
            {
                _logger.LogInformation($" Delete a video with videoId {id}  failed message: {ex}");

                return StatusCode(500, new { message = "Internal Server Error" });
            }


        }

        //Öğretmenin belirli bir kurstan belirli bir video güncelleme işlemini yapar. 
        [HttpPut("UpdateVideoFromCourse/{id}")]
        public async Task<ActionResult<Video>> UpdateVideoFromCourse(Guid id, [FromForm] UpdateVideoViewModel video)
        {
            try
            {
                if (isRoleTeacher())
                {
                    var existingVideo = await _videoRepository.GetByIdAsync(id);

                    if (existingVideo == null)
                    {
                        return NotFound("No video found to update");
                    }

                    // Yeni dosyayı yükle
                    var uploadedFileResult = await _fileStorageService.UploadFileAsync(video.File, id);

                    if (uploadedFileResult == null)
                    {
                        existingVideo.Title = video.Title;
                    }

                    if (uploadedFileResult != null)
                    {
                        existingVideo.Title = video.Title;
                        existingVideo.Url = uploadedFileResult.Url;
                    }



                    await _videoRepository.UpdateAsync(existingVideo);
                    _logger.LogInformation($" Update a video with videoId {id}  successful");


                    return Ok();
                }

                return BadRequest("Invalid Role");
            }
            catch (Exception ex)
            {
                _logger.LogInformation($" Update a video with videoId {id}  failed message: {ex}");

                return StatusCode(500, new { message = "Internal Server Error" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> getVideoById(Guid videoId)
        {
            var response = await _videoRepository.GetByIdAsync(videoId);
            return Ok(response);
        }


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
