using System.ComponentModel.DataAnnotations;

namespace SchoolHub.Entity;

public class ClassEntity
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public int SchoolId { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public int GradeId { get; set; }

}
