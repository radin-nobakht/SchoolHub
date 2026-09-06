using SchoolHub.Entity;

namespace SchoolHub.Dto.Manager
{
    public class AssignSubjectDto
    {
        public List<GeneralItemDto> AvailableSubjects { get; set; } = new();
        public List<int> TeacherSubjectIds { get; set; } = new();
    }
}
