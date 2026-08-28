using SchoolHub.Dto.School;

namespace SchoolHub.Dto.Manager
{
    public class ManagerClassInfoDto
    {

        public int ClassId { get; set; }

        public int SchoolId { get; set; }

        public string SchoolName { get; set; } = null!;

        public string ClassName { get; set; } = null!;

        public string GradeName { get; set; } = null!;

        public string? MajorName { get; set; }

        public double Average { get; set; }

        public List<ManagerClassStudentDto> Students { get; set; } = [];

        public List<ManagerClassTeacherDto> Teachers { get; set; } = [];
        public ClassDto Class { get; set; } = new();
    }
}
