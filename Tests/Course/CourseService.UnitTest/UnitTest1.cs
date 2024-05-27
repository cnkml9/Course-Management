using AutoMapper;
using CourseService.API.Controllers;
using CourseService.Application.Abstraction.Repositories;
using CourseService.Application.Abstraction.Services;
using CourseService.Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CourseService.UnitTest
{
    public class UnitTest1
    {
        [TestClass]
        public class CourseControllerTests
        {
            private Mock<ICourseRepository> mockCourseRepository;
            private Mock<IMapper> mockMapper;
            private Mock<IWebHostEnvironment> mockWebHostEnvironment;
            private Mock<ILoggerService> mockLoggerService;

            [TestInitialize]
            public void Setup()
            {
                mockCourseRepository = new Mock<ICourseRepository>();
                mockMapper = new Mock<IMapper>();
                mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
                mockLoggerService = new Mock<ILoggerService>();
            }

            [TestMethod]
            public async Task ListAllCourse_ReturnsOkResult_WhenCoursesExist()
            {
                // Arrange
                var controller = new CourseController(
                    mockCourseRepository.Object,
                    mockMapper.Object,
                    mockWebHostEnvironment.Object,
                    mockLoggerService.Object
                );

                var courseId = Guid.Parse("08dc30da-50be-4f97-8fd5-9ed514e388fb");

                // Assuming you have set up mockCourseRepository to return a list of courses
                var mockCourses = new List<Course>
                {
                      new Course
                    {
                        Id = Guid.NewGuid(),
                        Title = "Sample Course 1",
                        Description = "Description for Sample Course 1",
                        Amount = 19.99m,
                        CreatedDate = DateTime.UtcNow,
                        TeacherId = Guid.NewGuid()
                    },

                };

                mockCourseRepository
                    .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())) // You might want to match the specific courseId
                    .ReturnsAsync(new Course { /* your course properties here */ });
                // Act

                var result =  controller.DownloadCourse(courseId);

                            // Assert
                            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
                            var model = Xunit.Assert.IsAssignableFrom<IEnumerable<Course>>(okResult.Value);

                            // Add additional assertions based on your requirements
                            Xunit.Assert.NotNull(model);
                            Xunit.Assert.Equal(mockCourses.Count, model.Count());
                        }

        }

    }
}