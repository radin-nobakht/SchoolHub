using System.ComponentModel.DataAnnotations;

namespace SchoolHub.Entity;

public class DistrictEntity
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public int CityId { get; set; }

    [Required]
    public int District { get; set; }
}
