namespace SchoolHub.Dto.School
{
    public class AllDatailAboutSchool
    {
        public FullSchoolDataDto School { get; set; } = new FullSchoolDataDto();
        public List<ClassDto> Classes { get; set; } = new List<ClassDto>();
    }
}
