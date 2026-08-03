using System.ComponentModel.DataAnnotations;

namespace SchoolHub.Entity
{
    public class SubjectEntity
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Subject { get; set; }
    }
}
