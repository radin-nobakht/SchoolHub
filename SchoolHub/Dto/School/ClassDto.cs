namespace SchoolHub.Dto.School
{
    public class ClassDto
    {
        public int Id { get; set; }
        public int SchoolId { get; set; }
        public string Name { get; set; }
        public int GradeGeneralId { get; set; }
        public int? MajorGeneralId { get; set; }
        public string Grade { get; set; }
        public string Major { get; set; }
   

    }
}
