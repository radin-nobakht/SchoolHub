using SchoolHub.Dto.School;

namespace SchoolHub.Models
{
    public class MenuViewModel
    {
        public List<SchoolBarDto> MangerSchool { get; set; }
        public List<SchoolBarDto> TeacherSchool { get; set; }
    }
}
