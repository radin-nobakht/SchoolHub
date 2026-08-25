namespace SchoolHub.Dto.Teacher;

public class TeacherClassStudentDto
{
    public int StudentId { get; set; }

    public string FullName { get; set; } = string.Empty;
    public int Positives { get; set; }
    public int Negatives { get; set; }
    public double FirstTermGPA { get; set; }
    public double SecondTermGPA { get; set; }
}
