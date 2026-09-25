namespace SchoolHub.Dto.Manager
{
    public class ReportCardSubjectDto
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        public int ScoreCount { get; set; }
        public double AverageScore { get; set; }

        public int PositiveCount { get; set; }
        public int NegativeCount { get; set; }

        public List<ReportCardScoreDto> Scores { get; set; } = new();
    }
}
