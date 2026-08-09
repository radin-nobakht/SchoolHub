using SchoolHub.Entity;

namespace SchoolHub.Models
{
    public class AddClassViewModel
    {
        public List<GeneralItemDto> GeneralGrades { get; set; }

        public List<GeneralItemDto> GeneralMajors { get; set; }
    }
}
