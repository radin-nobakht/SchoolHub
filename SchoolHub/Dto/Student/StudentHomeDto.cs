using SchoolHub.Entity;

namespace SchoolHub.Dto.Student;

public class StudentHomeDto
{
    public StudentInfoDto StudentInfo { get; set; } = new();

    public SubjectInfoDto SubjectInfo { get; set; } = new();
    public List<GeneralItemDto> Subjects { get; set; } = new();
    public List<AverageScoresDto> Averages { get; set; } = new();
}
