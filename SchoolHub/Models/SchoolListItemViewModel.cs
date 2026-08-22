using SchoolHub.Dto.School;

namespace SchoolHub.Models
{
    public class SchoolListItemViewModel
    {
        public SchoolDto School { get; set; } = new();

        public bool IsManager { get; set; }
        public bool IsTeacher { get; set; }
        public bool IsStudent { get; set; }
    }
}
