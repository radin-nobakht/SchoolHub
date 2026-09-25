namespace SchoolHub.Dto.Manager;

public class StudentReportCardDto
{
    public int StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public string Grade { get; set; } = string.Empty;

    public string? Major { get; set; }

    public string Gender { get; set; } = string.Empty;

    public string SchoolName { get; set; } = string.Empty;

    public double OverallAverage { get; set; }

    public List<ReportCardSubjectDto> Subjects { get; set; } = [];
}