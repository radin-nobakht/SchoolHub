namespace SchoolHub.Models
{
    public class TeachersViewModel
    {
        public int SchoolId { get; set; }

        public string SchoolName { get; set; } = string.Empty;

        public List<TeacherListItemViewModel> Teachers { get; set; } = [];
    }

    public class TeacherListItemViewModel
    {
        public int TeacherId { get; set; }
        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string NationalId { get; set; } = string.Empty;
    }
}
