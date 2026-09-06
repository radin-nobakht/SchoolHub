namespace SchoolHub.Dto.Manager
{
    public class TeacherDto
    {
        public int TeacherUserId { get; set; }
        public int ClassId { get; set; }
        public List<int> SubjectIds { get; set; } = new List<int>();
    }
}
