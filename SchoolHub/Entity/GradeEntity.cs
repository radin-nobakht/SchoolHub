using System.ComponentModel.DataAnnotations;

namespace SchoolHub.Entity;

public class GradeEntity
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public int GradeNumber { get; set; }

    [Required]
    public int EducationLevelGeneralId { get; set; }

    [Required]
    public int EducationPeriodGeneralId { get; set; }
}
