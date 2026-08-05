namespace SchoolHub.Dto.School
{
    public class SchoolBarDto
    {
        public string SchoolType { get; set; } 
        public List<School> Schools { get; set; }
    }
    public class School
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
