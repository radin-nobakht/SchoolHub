namespace SchoolHub.Dto.School
{
    public class AllDatailAboutSchool
    {
        public SchoolDto School { get; set; }=new SchoolDto();
        public UserDto Manager { get; set; } = new UserDto();
        public List<SchoolInfoDto> Classes { get; set; }=new List<SchoolInfoDto>();
    }
    
    public class SchoolInfoDto
    {
        public ClassDto classes { get; set; } = new ClassDto();
        public UserDto Teacher { get; set; } = new UserDto();
    }
}
