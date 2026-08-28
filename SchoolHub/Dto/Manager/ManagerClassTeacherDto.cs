using SchoolHub.Entity;

namespace SchoolHub.Dto.Manager
{
    public class ManagerClassTeacherDto
    {
        public int TeacherUserId { get; set; }

        public string FullName { get; set; } = null!;

        public string NationalIdNumber { get; set; } = string.Empty;

        public List<GeneralItemDto> Subjects { get; set; } = [];
    }
}
