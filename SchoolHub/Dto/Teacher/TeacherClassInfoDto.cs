namespace SchoolHub.Dto.Teacher;

public class TeacherClassInfoDto
{
    public int ClassId { get; set; }

    public string ClassName { get; set; } = string.Empty;

    public string SchoolName { get; set; } = string.Empty;

    public string Grade { get; set; } = string.Empty;

    public string? Major { get; set; }

    public List<TeacherClassSubjectDto> Subjects { get; set; } = [];

    public List<TeacherClassStudentDto> Students { get; set; } = [];
}
