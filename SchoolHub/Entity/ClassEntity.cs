using System.ComponentModel.DataAnnotations;

namespace SchoolHub.Entity
{
    public class ClassEntity
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string ClassName { get; set; }

        [Required]
        public string Grade { get; set; }

        [Required]
        public string TecherUserId {  get; set; }
    }
}
