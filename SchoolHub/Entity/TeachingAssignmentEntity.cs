using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SchoolHub.Entity
{
    [Keyless]
    public class TeachingAssignmentEntity
    {
        [Required]
        public int ClassId { get; set; }

        public int TeacherUserId { get; set; }

        public int SubjectId { get; set; }
    }
}
