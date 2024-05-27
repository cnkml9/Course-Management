using CourseService.Application.Abstraction.Services;
using CourseService.Application.DTOs.Videos;

namespace CourseService.API.Services.Video
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public FileStorageService(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }


        public async Task<FileUploadResult> UploadFileAsync(IFormFile file,Guid courseId)
        {


            // Projenin çalışma dizini (wwwroot dahil) 
            var webRootPath = _hostingEnvironment.WebRootPath;

            // img klasörünün yolu
            var imgFolderPath = Path.Combine(webRootPath, courseId.ToString());

            // img klasörü yoksa oluştur
            if (!Directory.Exists(imgFolderPath))
            {
                Directory.CreateDirectory(imgFolderPath);
            }


            if (file == null || file.Length == 0)
            {
                return null; // Geçersiz dosya
            }

            // Dosya adı benzersiz bir şekilde oluşturulur
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(imgFolderPath, fileName);

            // Dosya yükleme işlemi gerçekleştirilir
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Dosya URL'si oluşturulur (istemciye dönmek için)
            var fileUrl = $"http://localhost:5001/{courseId}/{fileName}";

            return new FileUploadResult
            {
                FileName = fileName,
                Url = fileUrl
            };
        }
    }

}
