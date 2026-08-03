using System.ComponentModel.DataAnnotations;

namespace SchoolHub.Dto;

public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    [Required]
    public string NationalIdNumber { get; set; }
    [Required]
    public string Password { get; set; }
    public bool IsStudent { get; set; }
    public ValidationDto Validation { get; set; } = new ValidationDto();
}
