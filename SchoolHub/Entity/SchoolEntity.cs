using System.ComponentModel.DataAnnotations;

namespace SchoolHub.Entity
{
    public class SchoolEntity
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string SchoolName { get; set; }

        [Required]
        public string SchoolCity { get; set; }

        [Required]
        public int SchoolRegoin { get; set; }

        [Required]
        public int ManagerUserId { get; set; }
    }
}
