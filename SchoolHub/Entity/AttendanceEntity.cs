using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SchoolHub.Entity
{
    [Keyless]
    public class AttendanceEntity
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        [Required]
        public bool IsAbsent { get; set; }
    }
}
