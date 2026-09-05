using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SchoolHub.Entity
{
    [Keyless]
    public class TeachingAssignmentEntity
    {
        [Required]
        [Key]
        public int ClassId { get; set; }

        [Key]
        public int TeacherUserId { get; set; }

        [Key]
        public int SubjectId { get; set; }
    }
}
