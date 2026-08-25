using System.ComponentModel.DataAnnotations;

namespace SchoolHub.Entity
{
    public class GeneralItemEntity
    {
        [Key]
        [Required]
        public int Id { get; set; }

        public int? ParentId { get; set; }

        [Required]
        public string TitleType { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public bool Active { get; set; }

        public string? Description { get; set; }
    }
}
