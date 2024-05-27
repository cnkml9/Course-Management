namespace CourseService.Domain.Models
{
    public class Video
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public Guid CourseId { get; set; }

        public virtual Course Course { get; set; }
    }
}