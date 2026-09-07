namespace SchoolHub.Dto.Student;

public class StudentInfoDto
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = string.Empty;

    public string Grade { get; set; } = string.Empty;

    public int ClassId { get; set; }

    public string? Major { get; set; }

    public string Gender { get; set; } = string.Empty;

    public string SchoolName { get; set; } = string.Empty;
}
