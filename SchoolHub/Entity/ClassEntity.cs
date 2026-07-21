using System.ComponentModel.DataAnnotations;

namespace SchoolHub.Entity
{
    public class ClassEntity
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public int SchoolId { get; set; }

        [Required]
        public string ClassSubject { get; set; }

        [Required]
        public string Grade { get; set; }

        [Required]
        public int TecherUserId {  get; set; }
    }
}
