namespace SchoolHub.Dto.Student;

public class SubjectInfoDto
{
    public int SubjectId { get; set; }
    public string Name { get; set; } = string.Empty;

    public string TeacherName { get; set; } = string.Empty;

    public int ScoreCount { get; set; }

    public double AverageScore { get; set; }

    public int PositiveCount { get; set; }

    public int NegativeCount { get; set; }

    public List<ScoreDto> Scores { get; set; } = new();
}
