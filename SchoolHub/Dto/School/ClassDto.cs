namespace SchoolHub.Dto.School
{
    public class ClassDto
    {
        public int Id { get; set; }
        public int SchoolId { get; set; }
        public string ClassSubject { get; set; }
        public string Grade { get; set; }
        public int TecherUserId { get; set; }

    }
}
