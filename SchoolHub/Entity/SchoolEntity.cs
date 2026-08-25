using System.ComponentModel.DataAnnotations;

namespace SchoolHub.Entity
{
    public class SchoolEntity
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int ManagerUserId { get; set; }

        [Required]
        public int CityId { get; set; }

        public int DistrictId { get; set; }

        [Required]
        public int ShiftGeneralId { get; set; }

        [Required]
        public int TypeGeneralId { get; set; }

        [Required]
        public int GenderGeneralId { get; set; }

        [Required]
        public int EducationLevelGeneralId { get; set; }

        [Required]
        public int EducationPeriodGeneralId { get; set; }
    }
}
