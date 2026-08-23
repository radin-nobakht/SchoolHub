using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SchoolHub.Entity
{
    [Keyless]
    public class ScoreEntity
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        public string Score { get; set; }

        [Required]
        public string Reason { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int GeneralSubjectId { get; set; }

    }
}
