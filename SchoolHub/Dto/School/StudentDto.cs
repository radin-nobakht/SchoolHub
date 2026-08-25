using System.ComponentModel.DataAnnotations;

namespace SchoolHub.Dto.School
{
    public class StudentDto
    {
        public int Id { get; set; }

        public int StudentUserId { get; set; }
        public string StudentFullName { get; set; } = string.Empty;

        public int ClassId { get; set; }

        public int? Positives { get; set; }

        public int? Negatives { get; set; }

        public decimal? FirstTermGPA { get; set; }

        public decimal? SecondTermGPA { get; set; }
    }
}
