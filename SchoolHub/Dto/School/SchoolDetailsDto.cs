namespace SchoolHub.Dto.School
{
    public class AllDatailAboutSchool
    {
        public SchoolDto School { get; set; }=new SchoolDto();
        public UserDto Manager { get; set; } = new UserDto();
        public List<ClassDto> Classes { get; set; }=new List<ClassDto>();
    }
    
  
}
