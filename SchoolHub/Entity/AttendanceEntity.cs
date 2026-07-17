using System.ComponentModel.DataAnnotations;

namespace SchoolHub.Entity
{
    public class AttendanceEntity
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        public DateOnly Date {  get; set; }

        [Required]
        public bool IsAbsent { get; set; }
    }
}
