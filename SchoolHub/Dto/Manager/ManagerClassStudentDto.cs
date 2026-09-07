namespace SchoolHub.Dto.Manager
{
    public class ManagerClassStudentDto
    {
        public int StudentId { get; set; }
        public int StudentUserId { get; set; }
        public string NationalIdNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        public string? Gender { get; set; }

        public double Average { get; set; }
    }
}
