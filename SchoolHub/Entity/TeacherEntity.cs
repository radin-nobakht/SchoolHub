using System.ComponentModel.DataAnnotations;

namespace SchoolHub.Entity
{
    public class TeacherEntity
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public int TeacherUserId { get; set; }

        [Required]
        public int SchoolId { get; set; }

        [Required]
        public bool Active { get; set; }
    }
}
