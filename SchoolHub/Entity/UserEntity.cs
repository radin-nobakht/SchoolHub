using System.ComponentModel.DataAnnotations;

namespace SchoolHub.Entity
{
    public class UserEntity
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string NationalIdNumber { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public bool IsStudent {  get; set; }

        [Required]
        public int GenderGeneralId {  get; set; }
    }
}
