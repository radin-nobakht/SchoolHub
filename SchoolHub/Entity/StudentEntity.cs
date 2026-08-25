using System.ComponentModel.DataAnnotations;

namespace SchoolHub.Entity
{
    public class StudentEntity
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public int StudentUserId { get; set; }

        [Required]
        public int ClassId { get; set; }

        public int? Positives { get; set; }

        public int? Negatives { get; set; }

        public decimal? FirstTermGPA { get; set; }

        public decimal? SecondTermGPA { get; set; }
    }
}
