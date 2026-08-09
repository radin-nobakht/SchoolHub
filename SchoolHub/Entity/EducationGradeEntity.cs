using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SchoolHub.Entity
{
    [Keyless]
    public class EducationGradeEntity
    {
        [Required]
        public int GeneralEducationLevelId { get; set; }

        [Required]
        public int GeneralEducationPeriodId { get; set; }

        [Required]
        public int GeneralGradeId { get; set; }
    }
}
